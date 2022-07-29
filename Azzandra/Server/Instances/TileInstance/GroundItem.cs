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
        public Item Item { get; private set; }
        public override bool RenderLightness => false;
        public override bool IsSolid() => false;
        public override bool IsInteractable() => true;
        public static Symbol Symbol = new Symbol('x', Color.Red);
        public override Symbol GetSymbol() => Symbol;
        public override string Name => Item?.ToString() ?? "null";
        public override string ToStringAdress() => "the " + Name;


        public GroundItem(int x, int y) : base(x, y) { }


        public GroundItem(int x, int y, Item item) : base(x, y)
        {
            Item = item;
            Item.Container = null;
        }
        

        public override void Load(byte[] bytes, ref int pos)
        {
            Item = Item.LoadUnknown(bytes, ref pos);

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = Item.ToBytesUnknown(Item);

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            // Check item not null
            if (Item == null)
            {
                player.User.Log.Add("<red>That item does not exist!");
                Destroy();
                return;
            }

            // Check player has inventory space
            if (player.User.Inventory.CanAddItem(Item))
            {
                player.User.Inventory.AddItem(Item);
                player.User.Log.Add("You pick up " + (Item.Quantity != 1 ? Item.ToString() : "the " + Item.ToString()) + ".");
                Destroy();
            }
            else
            {
                player.User.Log.Add("You don't have enough space in your inventory to hold that.");
            }
        }
    }
}
