using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class MultipleItemContainer : Instance
    {
        protected bool IsOpen = false;
        public Inventory Inventory { get; set; } = new Inventory();
        public bool HasItems => Inventory?.Items.Count() > 0;
        public override bool IsInteractable() => true;
        public override bool RenderLightness => false;

        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.White)
                : new Symbol('¢', Color.LightGray);
        }

        public MultipleItemContainer(int x, int y) : base(x, y) { }


        /// Saving & Loading:

        public override void Load(byte[] bytes, ref int pos)
        {
            IsOpen = BitConverter.ToBoolean(bytes, pos);
            pos += 4;
            
            int invBytesAmt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var invBytes = new byte[invBytesAmt];
            Array.Copy(bytes, pos, invBytes, 0, invBytesAmt);
            Inventory.Load(invBytes);
            pos += invBytesAmt;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[8];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(IsOpen));
            pos += 4;

            var invBytes = Inventory.ToBytes();
            int invBytesAmt = invBytes.Length;
            bytes.Insert(pos, BitConverter.GetBytes(invBytesAmt));
            pos += 4;

            return bytes.Concat(invBytes).Concat(base.ToBytes()).ToArray();
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            if (!IsOpen)
            {
                IsOpen = true;
                var msg = "<r>You open the " + Name + ". ";
                msg += HasItems ? "It contains: " + Inventory.Items.Stringify2(i => "<aqua>" + i.ToString() + "<r>") + "." : "It appears to be empty.";

                player.User.Log.Add(msg);
                return;
            }

            if (HasItems)
            {
                var added = new List<Item>();
                for (int i = 0; i < Inventory.Items.Count; i++)
                {
                    var item = Inventory.Items[i];
                    if (player.User.Inventory.CanAddItem(item))
                    {
                        player.User.Inventory.AddItem(item);
                        Inventory.RemoveItem(item);
                        added.Add(item);
                        i--;
                    }
                }

                if (added.Count == 1)
                    player.User.Log.Add("<gold>You take the " + added.First().ToString() + " from the " + Name + ".");
                else if (Inventory.Items.Count <= 0)
                    player.User.Log.Add("<gold>You take all the items from the " + Name + ".");
                else if (added.Count > 1)
                    player.User.Log.Add("<gold>You take some the items from the " + Name + ".");

                if (Inventory.Items.Count > 0)
                    player.User.Log.Add("<rose>You don't have enough space to hold: " + Inventory.Items.Stringify2(i => i.ToString()) + ".");

                //var item = Contents.Items.First();
                //if (player.User.Inventory.CanAddItem(item))
                //{
                //    player.User.Inventory.AddItem(item);
                //    player.User.Log.Add("<gold>You take the " + item.ToString() + " from the " + Name + ".");
                //    Contents.RemoveItem(item);
                //    return;
                //}
                //else
                //{
                //    player.User.Log.Add("<rose>You dont have enough space to hold the " + item.ToString() + ".");
                //    return;
                //}
            }
            else
                player.User.Log.Add("<r>The " + Name + " appears to be empty.");
        }
    }
}
