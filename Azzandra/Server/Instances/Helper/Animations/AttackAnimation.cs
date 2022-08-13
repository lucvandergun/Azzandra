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
        
        public AttackAnimation(Instance origin, Instance target)
        {
            Dist = (origin.GetAbsoluteStaticPos() - target.GetAbsoluteStaticPos());
            Dist.Normalize();
            Dist *= ViewHandler.GRID_SIZE / 3;
        }

        public Instance Owner => null;

        public Vector2 GetDisposition()
        {
            //float turnAround = 0.67f;
            //if (tickFrac < turnAround)
            //    return -Dist / turnAround * tickFrac;
            //else
            //    return -(Dist * ((1 - tickFrac) / (1 - turnAround)));
            return Vector2.Zero;
        }

        public void Update()
        {
            
        }
    }
}
