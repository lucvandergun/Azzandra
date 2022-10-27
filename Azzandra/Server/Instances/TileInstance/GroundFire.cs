
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class GroundFire : Instance
    {
        public override bool RenderLightness => false;
        public override bool RenderFire => true;
        public int Time = 12;

        public override Symbol GetSymbol() => new Symbol("*", Color.DarkOrange);
        public override string Name => "fire";
        public override string AssetName => "fire";

        public override bool IsSolid() => false;
        public override MoveType GetMovementType() => MoveType.Fly;
        //public override bool IsInstanceSolidToThis(Instance inst) => inst is Entity ? false : base.IsInstanceSolidToThis(inst);

        public override void OnInstanceCollision(Entity collider)
        {
            if (collider.AddStatusEffect(new StatusEffects.Burning(1, 6), true) && collider is Player player)
                player.User.ShowMessage("<orange>As you move through the fire you are set ablaze!");
        }

        public GroundFire(int x, int y) : base(x, y)
        {
            
        }

        public override void TurnStart()
        {
            base.TurnStart();

            Time--;
            if (Time <= 0)
                Destroy();
        }

        /// Saving & Loading:

        public override void Load(byte[] bytes, ref int pos)
        {
            Time = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(Time));
            pos += 4;

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
