using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class SingleItemContainer : Instance
    {
        protected bool IsOpen = false;
        public Item Item { get; set; }
        public override bool IsInteractable() => true;
        public override bool RenderLightness => false;

        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.White)
                : new Symbol('¢', Color.LightGray);
        }

        public SingleItemContainer(int x, int y) : base(x, y) { }


        /// Saving & Loading:

        public override void Load(byte[] bytes, ref int pos)
        {
            IsOpen = BitConverter.ToBoolean(bytes, pos);
            pos += 4;
            Item = Item.LoadUnknown(bytes, ref pos);

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(IsOpen));
            pos += 4;

            var itemBytes = Item.ToBytesUnknown(Item);

            return bytes.Concat(itemBytes).Concat(base.ToBytes()).ToArray();
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            if (!IsOpen)
            {
                IsOpen = true;
                var msg = "<r>You open the " + Name + ". ";
                msg += Item != null ? "It contains: <aqua>" + Item.ToString() + "<r>." : "It appears to be empty.";

                player.User.Log.Add(msg);
                return;
            }

            if (Item != null)
            {
                if (player.User.Inventory.CanAddItem(Item))
                {
                    player.User.Inventory.AddItem(Item);
                    player.User.Log.Add("<gold>You take the " + Item.ToString() + " from the " + Name + ".");
                    Item = null;
                    return;
                }
                else
                {
                    player.User.Log.Add("<rose>You dont have enough space to hold the " + Item.ToString() + ".");
                    return;
                }
            }
            else
                player.User.Log.Add("<r>The " + Name + " appears to be empty.");
        }
    }
}
