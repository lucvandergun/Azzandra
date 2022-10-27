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
        public override int Initiative => base.Initiative;
        public override bool RenderLightness => false;

        private readonly float _angle;
        private readonly Color _color;
        private readonly string _asset;

        public override float Angle => _angle;
        public override string AssetName => _asset;
        public override Color AssetLightness => _color;


        public TargetProjectileMoving(Instance origin, Instance target, Color color, string asset) : base(origin, target)
        {
            _angle = CalculateAngle();
            ActionPotential = -100;
            TimeSinceLastTurn = 0;
            _color = color;
            _asset = asset;
        }
        
        public override void TurnEnd()
        {
            Destroy();
        }


        //Rendering:
        public override Vector2 CalculateRealPos(Server server)
        {
            float tickFraction = Level.Server.GetTickFraction(MomentOfLastTurn, Initiative);
            tickFraction = 1 - tickFraction; // Because it is rendered FROM the coordinates of the Origin instance.

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
