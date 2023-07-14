using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Equipment : Item
    {
        public virtual int Slot { get; set; }
        public int Accuracy;
        public int Damage;
        public int Spellcast;
        public int Evade;
        public int Parry;
        public int Block;
        public int Armour;
        public int Resistance;
        public int Weight;

        public virtual bool CanBlock => false;
        public virtual bool CanParry => false;

        public override int GetThrowAcc() => Accuracy;
        public override int GetThrowDmg() => Damage;

        public Equipment() : base()
        {

        }

        public override void SetAttributes(Item reference)
        {
            if (reference is Equipment e)
            {
                Slot = e.Slot;
                Accuracy = e.Accuracy;
                Damage = e.Damage;
                Spellcast = e.Spellcast;
                Evade = e.Evade;
                Parry = e.Parry;
                Block = e.Block;
                Armour = e.Armour;
                Resistance = e.Resistance;
                Weight = e.Weight;
            }

            base.SetAttributes(reference);
        }


        public virtual string EquipAction => "equip";
        public override Color StringColor =>
            ID.Contains("iron") ? Color.Gray :
            ID.Contains("steel") ? Color.LightGray :
            ID.Contains("samarite") ? Color.Teal :
            ID.Contains("pine") ? Color.NavajoWhite:
            ID.Contains("oak") ? Color.BurlyWood :
            ID.Contains("yew") ? Color.Chocolate : // Peru
            ID.Contains("leather") ? Color.Sienna :
            ID.Contains("dragonscale") ? Color.DarkSeaGreen :
            Color.LightBlue;


        public override List<string> GetInfo()
        {
            var list = base.GetInfo();

            var allows = new List<string>();
            if (CanParry) allows.Add("parrying");
            if (CanBlock) allows.Add("blocking");
            if (allows.Count > 0) list.Add("Allows " + allows.Stringify2() + ".");

            var stats = ListStats();
            list.AddRange(stats);
            return list;
        }

        protected readonly string[] AttributeNames = new string[] { "Accuracy", "Damage", "Spellcast", "Evade", "Parry", "Block", "Armour", "Resistance" };
        protected int[] GetAttributes() => new int[] { Accuracy, Damage, Spellcast, Evade, Parry, Block, Armour, Resistance };
        public List<string> ListStats()
        {
            var attributes = GetAttributes();

            // For each attribute: compare value to that of existing slot.
            var currentAttributes = ((Equipment)User.Equipment.GetItemByIndex(Slot))?.GetAttributes() ?? new int[AttributeNames.Length];
            

            var stats = AttributeNames.Select((a, i) => a + ": <white>" + attributes[i] + "<r>" + 
                (attributes[i] == currentAttributes[i] ? "" :
                    " " + GetValueColorCode(attributes[i] - currentAttributes[i]) + "(" + (attributes[i] - currentAttributes[i]).GetSignString() + ")<r>"))
                .Where((a, i) => attributes[i] != 0 || currentAttributes[i] != 0);

            return stats.ToList();
        }


        //public string SubType { get; set; }

        public override List<string> GetOptions()
        {
            var options = new List<string>(2);

            if (Container != null && !(this is Weapon))
                if (Container is Inventory)
                    options.Add(EquipAction);

            options.AddRange(base.GetOptions());
            return options;
        }

        public override void PerformOption(string option)
        {
            switch (option)
            {
                case "equip":
                case "wield":
                case "wear":
                    Equip();
                    return;
            }

            base.PerformOption(option);
        }

        public void Equip(int? customSlot = null)
        {
            int slot = customSlot ?? Slot;

            // Check slot in bounds
            if (!User.Equipment.IsInBounds(slot))
            {
                User.ThrowError("Slot [" + slot + "] is out of equipment range");
                return;
            }

            // Check item is owned by player
            if (!(Container is UserInventory))
            {
                User.ThrowError("Item [" + ID + "] is not in your inventory.");
                return;
            }


            // Replace currently worn
            User.ShowMessage("You " + EquipAction + " " + (Quantity != 1 ? ToString() : "the " + ToString()) + ".", true);

            var currentlyEquipped = User.Equipment.Items[slot];
            Container.ReplaceItem(this, currentlyEquipped);
            User.Equipment.SetIndex(slot, this);

            // Update previous weapons
            if (slot == 0 || slot == 1)
                User.Equipment.WeaponSwap[slot] = currentlyEquipped;

            // Twohanders remove shields
            if (slot == 0 && this is Weapon weapon)
            {
                if (weapon.IsTwoHander)
                {
                    var currentShield = User.Equipment.Items[1];
                    if (currentShield != null)
                    {
                        User.Equipment.SetIndex(1, null);
                        User.Inventory.AddItem(currentShield);
                        
                        User.Equipment.WeaponSwap[1] = currentShield;
                    }
                }
            }

            // Offhands remove twohanders
            if (slot == 1)
            {
                if (User.Equipment.Items[0] is Weapon currentWeapon)
                {
                    if (currentWeapon.IsTwoHander)
                    {
                        User.Equipment.SetIndex(0, null);
                        User.Inventory.AddItem(currentWeapon);

                        User.Equipment.WeaponSwap[0] = currentWeapon;
                    }
                }
            }
        }
    }
}
