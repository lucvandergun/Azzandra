using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CursedChest : LargeChest
    {
        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.Orange.ChangeBrightness(-0.3f))
                : new Symbol('¢', Color.Orange);
        }

        public override string Name => "large chest";
        public override string SecretName => "cursed chest";

        public CursedChest(int x, int y) : base(x, y) { }


        /// Saving & Loading:
        //public override void Load(byte[] bytes, ref int pos)
        //{
        //    IsOpen = BitConverter.ToBoolean(bytes, pos);
        //    pos += 1;

        //    base.Load(bytes, ref pos);
        //}

        //public override byte[] ToBytes()
        //{
        //    var bytes = BitConverter.GetBytes(IsOpen);

        //    return bytes.Concat(base.ToBytes()).ToArray();
        //}


        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            if (!IsOpen)
            {
                player.User.Log.Add("<gold>You open the chest. It contains bones that look alive!");
                Level.CreateInstance(new GiantSkeleton(X, Y));
                IsOpen = true;
            }
            
            base.Interact(entity);
        }
    }
}
