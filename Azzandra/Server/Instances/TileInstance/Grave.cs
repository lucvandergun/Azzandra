using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Grave : Instance
    {
        public bool IsEmpty = false;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return IsEmpty ? new Symbol('G', Color.Gray.ChangeBrightness(-0.5f))
                : new Symbol('G', Color.Gray);
        }

        public override string AssetName => "grave2";
        public override Color AssetLightness => IsEmpty ? Color.Gray : Color.White;

        public Grave(int x, int y) : base(x, y) { }


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            IsEmpty = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(IsEmpty);

            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            if (!IsEmpty)
            {
                player.User.ShowMessage("<tan>You examine the grave. It seems to have been preserved rather well.");
            }
            else
            {
                player.User.ShowMessage("<tan>You take a look at the grave and see that it's empty.");
            }
        }
    }
}
