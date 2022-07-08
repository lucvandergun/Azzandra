using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Arrow : Projectile
    {
        Vector OriginPos, TargetPos;

        public Arrow(Vector p1, Vector p2) : base(p2.X, p2.Y)
        {
            OriginPos = p1;
            TargetPos = p2;
        }

        public override void Tick()
        {
            Destroy();
        }


        // Rendering:

        public override Vector2 CalculateRealPos()
        {
            float turnDelay = Program.Engine.TurnDelay;
            var fullMovement = (TargetPos - OriginPos).ToFloat() * Engine.GRID_SIZE;
            var tickFraction = turnDelay / Engine.TURN_SPEED;
            var tickFractionOffset = -fullMovement * tickFraction;

            return base.CalculateRealPos() + tickFractionOffset;
        }

        protected float CalculateAngle()
        {
            var realDist = (TargetPos - OriginPos).ToFloat() * Engine.GRID_SIZE;
            
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
