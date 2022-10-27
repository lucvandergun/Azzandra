using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AttackAnimation : IAnimation
    {
        public readonly Vector2 Dist;

        public int Duration;
        private Instance _owner;
        public Instance Owner => _owner;

        private int AnimationLength;
        private int MomentOfStart;

        public AttackAnimation(Instance origin, Instance target, int duration)
        {
            Dist = (origin.GetAbsoluteStaticPos() - target.GetAbsoluteStaticPos());
            Dist.Normalize();
            Dist *= ViewHandler.GRID_SIZE / 3;

            Duration = duration;
            _owner = origin;

            MomentOfStart = Owner.Level.Server.AmtUpdates;
            AnimationLength = Duration * Server.TICK_SPEED;
        }

        public Vector2 GetDisposition()
        {
            var animationFraction = Math.Max(0, 1f - ((float)Owner.Level.Server.AmtUpdates - MomentOfStart) / AnimationLength);
            float turnAround = 0.67f;
            if (animationFraction < turnAround)
                return -Dist / turnAround * animationFraction;
            else
                return -(Dist * ((1 - animationFraction) / (1 - turnAround)));
            //return Vector2.Zero;
        }

        public void Update()
        {
            Duration--;
            if (Duration <= 0)
                Owner.Animations.Remove(this);
        }
    }
}
