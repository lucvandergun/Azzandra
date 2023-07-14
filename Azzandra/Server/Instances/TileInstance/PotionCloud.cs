
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class PotionCloud : Instance
    {
        public override bool RenderLightness => false;
        public int Time = 1;

        public override Symbol GetSymbol() => new Symbol("@", Color.Purple);
        public override string Name => "potion cloud";
        public override string AssetName => "cloud";
        public override Color AssetLightness => Color.YellowGreen;

        public FoodEffect[] Effects;

        public override bool IsSolid() => false;
        public override MoveType StartingMoveType => MoveType.Fly;

        public override void OnCollisionWithInstance(Instance inst)
        {
            if (!(inst is Entity entity))
                return;
            
            if (Effects == null) return;
            foreach (var effect in Effects)
            {
                effect.Apply(entity);

                //if (collider.AddStatusEffect(new StatusEffects.Burning(1, 6), true) && collider is Player player)
                //    player.User.ShowMessage("<orange>As you move through the fire you are set ablaze!");
            }
                
        }

        public PotionCloud(int x, int y, FoodEffect[] effects) : base(x, y)
        {
            Effects = effects;
        }

        public override void TurnStart()
        {
            base.TurnStart();

            // Place acid tile if acid cloud
            if (Effects?.Any(e => e.ID == "acid") ?? false)
            {
                Level.SetObject(Position, new Block(BlockID.Acid));
            }

            Time--;
            if (Time <= 0)
                DestroyNextTurn();
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
