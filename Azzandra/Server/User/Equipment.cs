using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Equipment : Container
    {
        private static readonly string[] SpeedName = { "fast", "slow", "very slow", "extremely slow", "plain sluggish" };//{ "very fast", "fast", "average", "slow", "very slow", "extremely slow", "incredibly slow" };
        public static string GetSpeedName(int speed)
        {
            speed = Math.Min(Math.Max(1, speed), 7);
            return SpeedName[speed - 1];
        }
        
        
        private readonly User User;
        public const int MAX_ITEMS = 10;
        public bool IsInBounds(int index) => index >= 0 && index < MAX_ITEMS;

        public Item[] Items { get; private set; }
        public override IEnumerable<Item> GetItems()
        {
            return Items;
        }
        public Item[] WeaponSwap { get; private set; }
        public void RecomputeWeaponSwap(Item returnedMain, Item returnedOff)
        {
            if (returnedMain is Items.Weapon returnedWep && returnedWep.IsTwoHander)
            {
                WeaponSwap[0] = returnedMain;
                WeaponSwap[1] = null;
                return;
            }
            else if (returnedOff != null && WeaponSwap[0] is Items.Weapon prevWep && prevWep.IsTwoHander)
            {
                WeaponSwap[0] = null;
            }
            if (returnedMain != null)
                WeaponSwap[0] = returnedMain;
            if (returnedOff != null)
                WeaponSwap[1] = returnedOff;
        }

        public void SetCurrentWeaponsToSwap()
        {
            WeaponSwap = new Item[] { Items[0], Items[1] };
        }

        public int Accuracy { get; private set; }
        public int Damage { get; private set; }
        public int Spellcast { get; private set; }
        public int Evade { get; private set; }
        public int Parry { get; private set; }
        public int Block { get; private set; }
        public int Armour { get; private set; }
        public int Resistance { get; private set; }
        public int Weight { get; private set; }

        public Style AttackStyle { get; private set; }
        public int AttackRange { get; private set; }
        public int AttackSpeed { get; private set; }
        public string Weapon { get; private set; }

        public Item GetWeapon() => Items[0];
        public Items.AmmunitionType? GetRequiredAmmunitionType()
        {
            if (GetWeapon() is Items.RangedWeapon rWep)
            {
                return rWep.AmmunitionType;
            }
            else return null;
        }
        public int GetAmmunitionStrength()
        {
            var type = GetRequiredAmmunitionType();
            if (type == null) return 0;
            var item = User.Inventory.GetAmmunitionByType(type.Value);
            if (item == null) return 0;
            return ((global::Azzandra.Items.Ammunition)item).Damage;
        }

        public Equipment(User user)
        {
            User = user;
            Items = new Item[MAX_ITEMS];
            WeaponSwap = new Item[2];
            CalculateBonusses();
        }

        public override void RemoveItem(Item item)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == item)
                {
                    Items[i] = null;

                    if (i == 0 || i == 1)
                    {
                        WeaponSwap[i] = item;
                        //if (!(WeaponSwap[0] is Items.Weapon w && w.IsTwoHander))    
                    }

                    CalculateBonusses();
                    return;
                }
            }

            User.ThrowDebug("Could not find and remove " + item.ToString2() + " in equipment");
        }

        public override bool HasItem(Func<Item, bool> predicate)
        {
            return false;
        }

        public void SetIndex(int index, Item item)
        {
            if (IsInBounds(index))
            {
                Items[index] = item;

                if (item != null)
                    item.Container = this;

                CalculateBonusses();
            }
        }

        public override bool AddItem(Item item)
        {
            if (!(item is Items.Equipment equipItem))
            {
                User.ThrowDebug("Item " + item.Name + " is not equippable");
                return false;
            }

            SetIndex(equipItem.Slot, equipItem);
            item.Container = this;
            return true;
        }

        public override void ReplaceItem(Item item1, Item item2)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == item1)
                {
                    Items[i] = item2;
                    item2.Container = this;
                    CalculateBonusses();
                    return;
                }
            }

            User.ThrowDebug("Could not find " + item1.ToString() + " to replace with " + item2.ToString());
        }

        public override bool CanAddItem(Item item)
        {
            if (!(item is Items.Equipment equipItem))
                return false;

            /*
            if (item.IsStackable)
            {
                foreach (var invItem in Items)
                {
                    if (invItem.GetType() == item.GetType())
                    {
                        //add as much quantity as possible
                        if (invItem.Amount + item.Amount <= invItem.Amount)
                            return false;
                        else
                            return true;
                    }
                }
            }

            return Items.Count < MAX_ITEMS;
            */
            return true;
        }

        public override void Clear()
        {
            for (int i = 0; i < MAX_ITEMS; i++)
            {
                Items[i] = null;
            }

            WeaponSwap.Populate(null);
        }

        public void CalculateBonusses()
        {
            Accuracy = 0;
            Damage = 0;
            Spellcast = 0;
            Evade = 0;
            Parry = 0;
            Block = 0;
            Armour = 0;
            Resistance = 0;
            Weight = 0;

            Weapon = "bare hands";

            foreach (var item in Items)
            {
                //if empty slot
                if (item == null)
                    continue;

                //check if item is equipment
                if (item is Items.Equipment eq)
                {
                    //if current item is offhand weapon and there is a main hand weapon with a different style - skip it
                    if (!(eq == Items[1] && eq is Items.Weapon wep2 && Items[0] is Items.Weapon wep1 && wep1.Style != wep2.Style))
                    {
                        Accuracy += eq.Accuracy;
                        Damage += eq.Damage;
                    }
                    Spellcast += eq.Spellcast;
                    Evade += eq.Evade;
                    Parry += eq.Parry;
                    Block += eq.Block;
                    Armour += eq.Armour;
                    Resistance += eq.Resistance;
                    Weight += eq.Weight;
                }
            }

            //set attacking properties
            if (Items[0] != null)
            {
                if (Items[0] is Items.Weapon weapon)
                {
                    AttackStyle = weapon.Style;
                    AttackRange = weapon.Range;
                    AttackSpeed = weapon.Speed;
                    Weapon = weapon.Name;
                }
                else
                {
                    AttackStyle = Style.Melee;
                    AttackRange = 1;
                    AttackSpeed = 1;
                    Weapon = Items[0].Name;
                }
            }
            else
            {
                AttackStyle = Style.Melee;
                AttackRange = 1;
                AttackSpeed = 1;
                Weapon = "Bare hands";
            }
        }


        public override Item GetItemByIndex(int index)
        {
            if (index >= 0 && index < Items.Length)
                return Items[index];
            else
                return null;
        }
        public override int GetIndex(Item item) => Items.ToList().IndexOf(item);

        /// <summary>
        /// Checks whether the main or offhand slot have an item that allows parrying.
        /// </summary>
        /// <returns></returns>
        public bool CanParry()
        {
            return ((Items[0] as Items.Equipment)?.CanParry ?? false) || ((Items[1] as Items.Equipment)?.CanParry ?? false);
        }

        /// <summary>
        /// Checks whether the main or offhand slot have an item that allows blocking.
        /// </summary>
        /// <returns></returns>
        public bool CanBlock()
        {
            return ((Items[0] as Items.Equipment)?.CanBlock ?? false) || ((Items[1] as Items.Equipment)?.CanBlock ?? false);
        }


        public List<AttackProperty> GetAttackProperties()
        {
            var list = new List<AttackProperty>();

            Style mainStyle = Style.Other;
            if (Items[0] is Items.Weapon mainhand)
            {
                mainStyle = mainhand.Style;
                if (mainhand.AttackProperties != null)
                    list.AddRange(mainhand.AttackProperties);
            }

            if (Items[1] is Items.Weapon offhand && offhand.Style == mainStyle)
            {
                if (offhand.AttackProperties != null)
                    list.AddRange(offhand.AttackProperties);
            }

            // TODO: add ammunition attack properties:
            //if ()

            return list;
        }




        /// <summary>
        /// Creates a byte representation of the equipment state
        /// </summary>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        /// <returns>byte array</returns>
        public byte[] ToBytes()
        {
            var bytes = new byte[0];
            int amt = Items.Length;
            bytes = bytes.Concat(BitConverter.GetBytes(amt)).ToArray();

            for (int i = 0; i < amt; i++)
            {
                var isFilled = Items[i] != null;    // specify whether slot is filled
                var filledBytes = BitConverter.GetBytes(isFilled);
                bytes = bytes.Concat(filledBytes).ToArray();

                if (isFilled) // only add item when filled
                {
                    var itemBytes = Items[i].ToBytes();
                    bytes = bytes.Concat(itemBytes).ToArray();
                }
            }

            // Swaps:
            for (int i = 0; i < 2; i++)
            {
                bytes = bytes.Concat(BitConverter.GetBytes(User.Inventory.Items.IndexOf(WeaponSwap[i]))).ToArray();
            }

            //Debug.WriteLine("Saved " + amt + " eq items.");
            return bytes;
        }

        /// <summary>
        /// Loads the equipment from a given byte array
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
                bool isFilled = BitConverter.ToBoolean(bytes, pos);
                pos += 1;

                if (isFilled)
                {
                    var item = Item.Load(bytes, ref pos);
                    if (item != null)
                        SetIndex(i, item);
                    else
                        success = false;
                }
            }

            // Swaps:
            for (int i = 0; i < 2; i++)
            {
                var index = BitConverter.ToInt32(bytes, pos);
                pos += 4;
                WeaponSwap[i] = User.Inventory.GetItemByIndex(index);
            }

            //Debug.WriteLine("Loaded " + amt + " eq items.");
            return success;
        }
    }
}
