using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Kobold : Enemy
    {
        public override EntityType EntityType => EntityType.Humanoid;
        public override bool CanOpenDoors() => true;
        public Inventory Inventory { get; set; } = new Inventory();
        public bool HasItem() => Inventory.Items.Count > 0;

        public override Symbol GetSymbol() => new Symbol('k', Color.Firebrick);



        public Kobold(int x, int y) : base(x, y) { }

        protected override void DropItemsOnDeath()
        {
            foreach (var item in Inventory.Items)
                DropItem(item);
        }



        public override EntityAction DetermineAggressiveAction()
        {
            if (HasItem())
            {
                // Flee
                return new ActionFlee(this, Target.Instance);// ActionMove(this, (Position - Target.Instance.Position).Sign());
            }
            else
            {
                // Try to take item
                if (Target.Instance is Player player && player.User.Inventory.Items.Count > 0)
                {
                    if (IsTouchingOrColliding(player))
                    {
                        var item = player.User.Inventory.Items.PickRandom();
                        player.User.Inventory.RemoveItem(item);
                        Inventory.AddItem(item);
                        player.User.ShowMessage("<rose>" + ToStringAdress().CapFirst() + " has stolen " + item.ToString3() + " from you!");
                        return null;
                    }
                    else return new ActionMoveTo(this, player);
                }

                // Just attack
                else
                {
                    return base.DetermineAggressiveAction();
                }
            }
        }



        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
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
            var bytes = new byte[4];
            int pos = 0;

            var invBytes = Inventory.ToBytes();
            int invBytesAmt = invBytes.Length;
            bytes.Insert(pos, BitConverter.GetBytes(invBytesAmt));
            pos += 4;

            return bytes.Concat(invBytes).Concat(base.ToBytes()).ToArray();
        }


        // === Rendering === \\
        public override void Draw(SpriteBatch sb, Vector2 pos, float lightness = 1f)
        {
            base.Draw(sb, pos, lightness);
            
            if (Inventory.Items.Count > 0)
            {
                var asset = GroundItem.Asset;
                var itemOffset = new Vector2((GetW() - 1) * GameClient.GRID_SIZE / 4 + 3);
                new AnimationManager(asset).Draw(sb, pos + itemOffset);
            }
        }
    }
}
