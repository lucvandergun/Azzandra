using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;

namespace Azzandra
{
    public class Inventory : Container
    {
        public int MaxItems = 10;
        public List<Item> Items { get; private set; }
        public override IEnumerable<Item> GetItems()
        {
            return Items;
        }

        public Inventory()
        {
            Items = new List<Item>();
        }

        public override void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        public override bool AddItem(Item item)
        {
            if (item == null)
                return false;

            item.Container = this;

            if (item.Stack)
            {
                foreach (var invItem in Items)
                {
                    if (invItem.ID == item.ID)
                    {
                        // Add as much quantity as possible
                        if (invItem.Quantity + item.Quantity < invItem.Quantity)
                            invItem.Quantity = int.MaxValue;
                        else
                            invItem.Quantity += item.Quantity;

                        return true;
                    }
                }
            }

            Items.Add(item);
            //pseudo code for separating unstackables with quantity > 1:
            // if (!stackable) repeat amount add new item(1);

            return true;
        }

        public bool AddItems(IEnumerable<Item> items)
        {
            if (items == null)
                return false;

            bool failed = false;
            foreach (var item in items)
            {
                if (!AddItem(item))
                    failed = true;
            }

            return !failed;
        }

        public override void ReplaceItem(Item item1, Item item2)
        {
            if (item1 == null)
            {
                return;
            }
            else if (item2 == null)
            {
                Items.Remove(item1);
                return;
            }


            var index = Items.IndexOf(item1);
            if (index == -1)
            {
                //User.ThrowDebug("Could not find " + item1.ToString() + " to replace with " + item2.ToString());
                return;
            }

            Items[index] = item2;
            item2.Container = this;
        }

        public override bool CanAddItem(Item item)
        {
            if (item == null)
                return true;

            if (item.Stack)
            {
                foreach (var invItem in Items)
                {
                    if (invItem.GetType() == item.GetType())
                    {
                        // Check whether slot holds quantity
                        if (invItem.Quantity + item.Quantity <= invItem.Quantity)
                            return false;
                        else
                            return true;
                    }
                }
            }

            return Items.Count < MaxItems;
        }

        public bool CanAddItems(IEnumerable<Item> items)
        {
            if (items == null)
                return true;

            int amtSpacesUsed = 0;
            foreach (var item in items)
            {
                if (item.Stack)
                {
                    foreach (var invItem in Items)
                    {
                        if (invItem.GetType() == item.GetType())
                        {
                            // Check whether overflow happens:
                            if (invItem.Quantity + item.Quantity <= invItem.Quantity)
                                return false;
                        }
                        else
                            amtSpacesUsed++;
                    }
                }
            }
            

            return amtSpacesUsed < MaxItems - Items.Count();
        }

        public override bool HasItem(Func<Item, bool> predicate)
        {
            foreach (var item in Items)
            {
                if (predicate.Invoke(item))
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveItem(Func<Item, bool> predicate, int amount)
        {
            foreach (var item in Items)
            {
                if (predicate.Invoke(item))
                {
                    item.RemoveAmount(amount);
                    return;
                }
            }
        }


        public override void Clear()
        {
            Items.Clear();
        }


        public Item CheckItemType(Type itemType, uint amount = 1)
        {
            foreach (var item in Items)
            {
                if (item.GetType() == itemType && item.Quantity >= amount)
                {
                    return item;
                }
            }

            return null;
        }

        public override Item GetItemByIndex(int index)
        {
            if (index >= 0 && index < Items.Count)
                return Items[index];
            else
                return null;
        }
        public override int GetIndex(Item item) => Items.IndexOf(item);

        public void MoveItem(Item item, int newIndex)
        {
            //boundarize index
            newIndex = Math.Min(Items.Count - 1, Math.Max(0, newIndex));

            Items.Remove(item);
            Items.Insert(newIndex, item);
        }


        public Item GetAmmunitionByType(Items.AmmunitionType type)
        {
            foreach (var item in Items)
            {
                if (item is Items.IAmmunition ammo)
                    if (ammo.AmmunitionType == type)
                        return item;
            }

            return null;
        }

        public bool HasAmmunitionType(Items.AmmunitionType type)
        {
            return GetAmmunitionByType(type) != null;
        }



  


        /// <summary>
        /// Creates a byte representation of the inventory state
        /// </summary>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <returns>byte array</returns>
        public byte[] ToBytes()
        {
            var bytes = new byte[0];
            int amt = Items.Count;
            bytes = bytes.Concat(BitConverter.GetBytes(amt)).ToArray();

            for (int i = 0; i < amt; i++)
            {
                var itemBytes = Items[i].ToBytes();
                
                bytes = bytes.Concat(itemBytes).ToArray();
            }
            
            //Debug.WriteLine("Saved " + amt + " inv items.");
            return bytes;
        }

        /// <summary>
        /// Loads the inventory from a given byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns>whether successful</returns>
        public bool Load(byte[] bytes)
        {
            Clear();
            
            int pos = 0;
            int amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            bool success = true;
            for (int i = 0; i < amt; i++)
            {
                var item = Item.Load(bytes, ref pos);

                if (item != null)
                    AddItem(item);
                else
                    success = false;
            }

            //Debug.WriteLine("Loaded " + amt + " inv items.");
            return success;
        }
    }
}
