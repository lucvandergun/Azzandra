using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionSwap : EntityAction
    {
        public readonly Item[] Items;

        public ActionSwap(Entity caller, Item[] items) : base(caller)
        {
            Items = items;
        }
        
        protected override bool PerformAction()
        {
            if (!(Caller is Player player))
                return false;
            
            if (Items == null || Items.All(i => i == null))
            {
                player.User.ShowMessage("<rose>You don't have any previously held weapons to swap to.");
                return false;
            }

            bool firstIsTwohander = Items.First() is Items.Weapon w && w.IsTwoHander;
            for (int i = 0; i < Items.Length; i++)
            {
                // Call 'equip'-action on the item[i]
                var item = Items[i];
                if (item is Items.Equipment eq && player.User.Inventory.Items.Contains(item))
                {
                    eq.Equip(i);
                }

                // Something went wrong (item not present in inventory or not equippable), remove from swapWeps list:
                else if (item != null)
                {
                    player.User.Equipment.WeaponSwap[i] = null;
                }

                // Stop if just equipped a two-handed item:
                if (firstIsTwohander)
                    break;
            }

            //player.User.ShowMessage("You swap to your secondary weapons.");
            return true;
        }
    }
}
