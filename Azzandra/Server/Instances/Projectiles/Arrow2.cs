using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Arrow2 : Projectile
    {
        Instance Origin, Target;
        Vector2 RealPos;

        public Arrow2(Instance i1, Instance i2) : base(i1.X, i1.Y)
        {
            Origin = i1;
            Target = i2;
            RealPos = GetCenter(Origin);
        }

        private Vector2 GetCenter(Instance i)
        {
            return i.CalculateRealPos();
        }
        

        public override void Tick()
        {
            Destroy();
        }


        // Rendering:

        public override Vector2 CalculateRealPos()
        {
            float turnDelay = Program.Engine.TurnDelay;

            var fullMovement = GetCenter(Target) - GetCenter(Origin);
            var tickFraction = 1 - turnDelay / Engine.TURN_SPEED;
            var movementFraction = fullMovement * tickFraction;

            return base.CalculateRealPos() + movementFraction;
        }

        protected float CalculateAngle()
        {
            var realDist = GetCenter(Target) - GetCenter(Origin);

            // Safety measure: return 'vertical' angle if dx == 0;
            if (realDist.X == 0)
            {
                return realDist.Y < 0
                    ? (270 / 180 * 3.1415f)
                    : (90 / 180 * 3.1415f);
            }

            var angle = (float)Math.Atan(realDist.Y / realDist.X);

            if (realDist.X < 0)
                angle -= 3.1415f;

            return angle - (3.1415f / 2);
        }

        public override void Draw(Vector2 pos)
        {
            var texture = Assets.Arrow;
            Display.DrawTextureRotated(pos, texture, Color.White, 1, CalculateAngle());
        }
    }
}
