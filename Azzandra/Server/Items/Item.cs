using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    [JsonConverter(typeof(ItemConverter))]
    public class Item
    {
        public static User User;
        public static int MaxQuantity => int.MaxValue;
        public int MaxAddable
        {
            get => MaxQuantity - Math.Min(Quantity, MaxQuantity);
        }

        public Container Container;

        
        // === SAVED ATTRIBUTES === \\
        
        public string ID { get; set; }
        
        private int _qty;
        public int Quantity
        {
            get => _qty;
            set
            {
                if (value <= 0)
                    Destroy();
                _qty = Math.Max(0, value);
            }
        }

        public int Durability { get; set; } = -1;


        // === DATA ATTRIBUTES === \\

        public string Name;
        public bool Stack = false;
        public string Plural;
        public string Desc;
        public string Asset;
        public int MaxDurability = -1;

        public virtual Color StringColor => Color.White;


        // === SPECIAL GETTERS === \\

        public virtual List<string> GetInfo()
        {
            return new List<string>();
        }

        public virtual string GetNameNotNull()
        {
            if (MaxDurability <= 0)
                return Name ?? "undefined";
            else
                return (Name ?? "undefined") + " [" + Durability + "]"; // "/" + MaxDurability +
        }

        public virtual string QuantityName(int qty)
        {
            return (qty > 1 && Plural != null) ? Plural : GetNameNotNull();
        }

        protected string GetValueColorCode(int value) => value > 0 ? "<lime>" : value < 0 ? "<red>" : "";
        protected string GetValueColorCode(int newValue, int oldValue)
        {
            return newValue > oldValue ? "<lime>" : newValue < oldValue ? "<red>" : "";
        }


        /// <summary>
        /// Returns the item class/type id code. This is the class name in all lower case.
        /// </summary>
        /// <returns></returns>
        public string GetTypeID() => GetType().Name.ToLower();

        /// <summary>
        /// Creates a string of format: "item[s] (qty)".
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Quantity != 1
                ? QuantityName(Quantity) + " (" + Util.StringifyNumber2(Quantity) + ")"
                : GetNameNotNull();
        }
        /// <summary>
        /// Creates a string of format: "qty x item".
        /// </summary>
        /// <returns></returns>
        public virtual string ToString2()
        {
            return Util.StringifyNumber2(Quantity) + " x " + GetNameNotNull();
        }
        /// <summary>
        /// Creates a string of format: "[a/an item]/[qty items]".
        /// </summary>
        /// <returns></returns>
        public virtual string ToString3()       // an/a single_name : qty multiple_name
        {
            return Quantity != 1
                ? Util.StringifyNumber2(Quantity) + " " + QuantityName(Quantity)
                : Util.AddArticle(Name);
        }

        public Item()
        {

        }


        // === MODIFIERS === \\

        public void Destroy()
        {
            //remove slot from container
            Container?.RemoveItem(this);
        }
        public void RemoveAmount(int amount)
        {
            if (amount >= Quantity)
            {
                Destroy();
            }
            else
            {
                Quantity -= amount;
            }
        }

        /// <summary>
        /// Returns this items' first option on the list. Returns null if none.
        /// </summary>
        /// <returns></returns>
        public string GetDefaultOption()
        {
            var options = GetOptions();
            return options == null ? null : options.Count <= 0 ? null : options[0];
        }
        public virtual List<string> GetOptions()
        {
            var options = new List<string>(3);

            if (Container != null)
                if (Container is Azzandra.Equipment)
                    options.Add("remove");
                if (Container is Azzandra.Inventory && this is Items.IFilledContainer)
                    options.Add("empty");

            if (Quantity > 1 && !(this is Items.Ammunition)) options.Add("drop one");
            options.Add("drop");
            return options;
        }


        public void PerformDefaultOption()
        {
            PerformOption(GetDefaultOption());
        }
        public virtual void PerformOption(string option)
        {
            switch (option)
            {
                default:
                    User.ThrowError("The item option \"" + option + "\" cannot be performed on item type \"" + GetType().Name + "\" (" + ID + ")");
                    return;
                
                case "examine":
                    User.Log.Add(Desc);
                    return;

                case "drop":
                    Container?.RemoveItem(this);
                    User.Player.DropItem(this);
                    User.ShowMessage("You drop " + (Quantity != 1 ? ToString() : "the " + ToString()) + ".", true);
                    return;
                case "drop one":
                    RemoveAmount(1);

                    var dropItem = Item.Create(ID, 1);
                    User.Player.DropItem(dropItem);

                    User.ShowMessage("You drop the " + dropItem.Name + ".", true);

                    return;

                case "remove":
                case "unequip":
                    Unequip();
                    return;

                case "empty":
                    Empty();
                    return;
            }
        }

        public void Empty()
        {
            if (this is Items.IFilledContainer container)
            {
                var emptyItem = container.EmptyItem;
                Container?.ReplaceItem(this, emptyItem);
                User.Log.Add("You empty the " + Name + ".");
            }
            else
                User.ThrowError("That item cannot be emptied.");
        }

        private void Unequip()
        {
            //check whether item is located in equipment
            if (Container != null)
                if (!(Container is Azzandra.Equipment))
                    User.ThrowError("You don't have the item you're trying to remove equipped.");

            if (User.Inventory.CanAddItem(this))
            {
                Container.RemoveItem(this);
                User.Inventory.AddItem(this);
                User.ShowMessage("You remove " + (Quantity != 1 ? ToString() : "the " + ToString()) + ".");
            }
            else
            {
                User.ShowMessage("<rose>You don't have enough space to hold that.");
            }
        }


        public void Replace(Item item)
        {
            if (Container == null)
            {
                User.ThrowError("The item to replace doesn't have a container");
                return;
            }

            Container.ReplaceItem(this, item);
        }



        /* Saving Format:
         * 
         * string(20)   id
         * int(4)       qty
         * ...          extra item(type) specific
         */

        public virtual byte[] ToBytes()
        {
            var bytes = new byte[20 + 4 + 4];

            bytes.Insert(0, GameSaver.GetBytes(ID));
            bytes.Insert(20, BitConverter.GetBytes(Quantity));
            bytes.Insert(24, BitConverter.GetBytes(Durability));

            return bytes;
        }

        public static Item Load(byte[] bytes, ref int pos)
        {
            var id = GameSaver.ToString(bytes, pos);
            pos += 20;
            var qty = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var durability = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            var item = Item.Create(id, qty);
            item.Durability = durability;
            return item;
        }

        public static byte[] ToBytesUnknown(Item item)
        {
            if (item == null)
                return BitConverter.GetBytes(false);
            else
                return BitConverter.GetBytes(true).Concat(item.ToBytes()).ToArray();
        }

        public static Item LoadUnknown(byte[] bytes, ref int pos)
        {
            bool isPresent = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            if (!isPresent)
                return null;
            else
            {
                var id = GameSaver.ToString(bytes, pos);
                pos += 20;
                var qty = BitConverter.ToInt32(bytes, pos);
                pos += 4;
                var durability = BitConverter.ToInt32(bytes, pos);
                pos += 4;

                var item = Item.Create(id, qty);
                item.Durability = durability;
                return item;
            }
        }


        public static Item Create(string id, int qty = 1)
        {
            var data = Data.GetItemData(id);
            var type = data.GetType();
            
            var item = (Item)Activator.CreateInstance(type);
            item.ID = id;
            item.Quantity = qty;
            item.SetAttributes(data);
            // Load durability
            item.Durability = item.MaxDurability;
            return item;
        }

        public virtual void SetAttributes(Item reference)
        {
            Name = reference.Name;
            Stack = reference.Stack;
            Plural = reference.Plural;
            Desc = reference.Desc;
            Asset = reference.Asset;
            MaxDurability = reference.MaxDurability;
        }


    }
}
