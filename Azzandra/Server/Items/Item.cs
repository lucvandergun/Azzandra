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

        public List<Property> Properties = new List<Property>();


        // === DATA ATTRIBUTES === \\

        public string Name;
        public bool Stack = false;
        public string Plural;
        public string Desc;
        public string Asset;
        public int MaxDurability = -1;

        public virtual Color StringColor => Color.White;


        // === SPECIAL GETTERS === \\

        public IEnumerable<AttackProperty> GetAttackProperties() => Properties.Where(p => p is AttackProperty).Select(p => (AttackProperty)p);
        public IEnumerable<FoodEffect> GetFoodEffects() => Properties.Where(p => p is FoodEffect).Select(p => (FoodEffect)p);

        public virtual List<string> GetInfo()
        {
            return new List<string>();
        }

        private bool DisplayProperty(Property p) => !p.IsHidden;
        public string GetPropertyInfo()
        {
            var props = Properties.Where(p => DisplayProperty(p));
            if (props.Count() > 0)
            {
                return "<lime>Effects: " + props.Select(a => a.ToString()).Stringify2() + "<r>";
            }

            return null;
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
            {
                if (Container is global::Azzandra.Equipment)
                    options.Add("remove");
                if (Container is global::Azzandra.Inventory)
                {
                    if (this is Items.IFilledContainer)
                        options.Add("empty");
                    if (this is Items.ILightable)
                        options.Add("light");
                }
                options.Add("throw");
            }

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

                case "throw":
                    Throw();
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
                case "light":
                    Light();
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

        public void Light()
        {
            if (this is Items.ILightable lightable)
            {
                lightable.Light();
            }
            else
                User.ShowMessage("You attempt to light it, but it doesn't catch fire.");
        }


        public void EmptyOnThrow()
        {
            if (this is Items.IFilledContainer container)
            {
                var emptyItem = container.EmptyItem;
                Container?.ReplaceItem(this, emptyItem);
                User.Log.Add("<tan>Its contents fell out in the process.");
            }
        }

        public void Throw()
        {
            var tt = new TargetingMode.TileTargeting();
            tt.InboundAction = new ActionThrow(User.Player, User.Player.Position, this);
            User.Server.GameClient.InputHandler.TargetingMode = tt;
        }

        private void Unequip()
        {
            //check whether item is located in equipment
            if (Container != null)
                if (!(Container is global::Azzandra.Equipment))
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

        /// <summary>
        /// Performs the on throw event, on a specific instance, for this particular item.
        /// Always call base method if it doesn't have an effect.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="inst"></param>
        /// <returns>Whether there was an effect, and the item is to be 'consumed', i.e. no longer checking for other instances/the floor.</returns>
        public virtual bool OnThrowOnInstance(Level level, GroundItem grit, Instance inst)
        {
            if (inst is Entity entity && entity.IsAttackable())
            {
                var attack = new Attack(User.Server, Style.Other, 1, 8, GetThrowAcc(), GetThrowDmg(), GetAttackProperties().ToList());
                var affect = User.Player.Affect(inst, attack);
                return affect.IsSuccess();
            }
            
            return false;
        }

        public virtual int GetThrowAcc() => 1;
        public virtual int GetThrowDmg() => 1;


        /// <summary>
        /// Performs the on throw event, on a specific location, for this particular item.
        /// Always call base method if it doesn't have an effect.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pos"></param>
        public virtual void OnThrowOnTile(Level level, GroundItem grit, Vector pos)
        {
            
        }


        public bool AddProperty(Property p)
        {
            if (Properties.Any(prop => prop.Equals(p)))
                return false;

            Properties.Add(p);
            return true;
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

            var propertiesBytes = GameSaver.SaveList(Properties, i => Property.Save(i));
            bytes = bytes.Concat(propertiesBytes).ToArray();

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

            // Properties
            var properties = GameLoader.LoadList(bytes, ref pos, i => Property.Load(i));

            var item = Item.Create(id, qty);
            item.Durability = durability;
            item.Properties = properties; // overrides default properties

            //User.ShowMessage("loaded props for "+item.Name+": " + properties.Stringify());
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
                return Item.Load(bytes, ref pos);
                //var id = GameSaver.ToString(bytes, pos);
                //pos += 20;
                //var qty = BitConverter.ToInt32(bytes, pos);
                //pos += 4;
                //var durability = BitConverter.ToInt32(bytes, pos);
                //pos += 4;

                //var item = Item.Create(id, qty);
                //item.Durability = durability;
                //return item;
            }
        }


        /// <summary>
        /// Used to create new Items from a given string ID.
        /// Should not be used when loading from a save file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Check whether two item stacks are stackable, they have to: 
        /// 1) be stackable, 
        /// 2) have the same ID, 
        /// 3) have the same Properties and in the same order.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsStackableWith(Item other)
        {
            if (!Stack || other.ID != ID)
                return false;

            if (Properties.Count != other.Properties.Count)
                return false;

            for (int i = 0; i < Math.Min(Properties.Count, other.Properties.Count); i++)
            {
                if (!Properties[i].Equals(other.Properties[i]))
                    return false;
            }

            return true;
        }
    }
}
