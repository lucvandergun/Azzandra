using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class TargetProjectileMoving : TargetProjectile
    {
        protected readonly float Angle;
        protected virtual Color Color => Color.White;


        public TargetProjectileMoving(Instance origin, Instance target) : base(origin, target)
        {
            Angle = CalculateAngle();
        }
        
        public override void TickEnd()
        {
            Destroy();
        }


        // Rendering:
        public override Vector2 CalculateRealPos(Server server)
        {
            float turnDelay = IsOnPlayerTick ? server.TurnDelay : server.EnemyTurnDelay;
            float tickFraction = 1 - turnDelay / Server.TURN_SPEED;

            Vector2 fullMovement = Target.CalculateRealPos(server) - Origin.CalculateRealPos(server);
            Vector2 movementFraction = fullMovement * tickFraction;

            return Origin.CalculateRealPos(server) + movementFraction;
            //return base.CalculateRealPos(server) + movementFraction;
        }

        protected float CalculateAngle()
        {
            var realDist = Target.GetAbsoluteStaticPos() - Origin.GetAbsoluteStaticPos();

            // Safety measure: return 'vertical' angle if dx == 0;
            if (realDist.X == 0)
            {
                return realDist.Y > 0
                    ? (270 / 180 * 3.1415f)
                    : (90 / 180 * 3.1415f);
            }

            var angle = (float)Math.Atan(realDist.Y / realDist.X);

            if (realDist.X > 0)
                angle -= 3.1415f;

            return angle - (3.1415f / 2);
        }
    }
}
