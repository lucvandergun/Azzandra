using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GroundItem : Instance
    {
        public Item.Generic Item { get; private set; }
        public override bool IsSolid() => false;
        public override bool IsInteractable() => true;
        public override Symbol GetSymbol() { return new Symbol('x', Color.Red); }
        public override string Name => Item.ToString();
        public override string ToStringAdress() => "the " + Name;


        public GroundItem(int x, int y, Item.Generic item) : base(x, y)
        {
            Item = item;
            Item.Container = null;
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            // TODO: Check player is on top of item
            if (!player.IsTouching(this) && !player.IsCollisionWith(this))
            {
                player.User.Log.Add("You are too far away to pick that up.");
                return;
            }

            // Check player has inventory space
            if (player.User.Inventory.CanAddItem(Item))
            {
                player.User.Inventory.AddItem(Item);
                player.User.Log.Add("You pick up " + (Item.Amount != 1 ? Item.ToString() : "the " + Item.ToString()) + ".");
                Destroy();
            }
            else
            {
                player.User.Log.Add("You don't have enough space in your inventory to hold that.");
            }
        }
    }
}
