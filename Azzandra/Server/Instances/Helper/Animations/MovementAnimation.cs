using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    ///// <summary>
    ///// A movement animation of only one direct step.
    ///// </summary>
    //public class MovementAnimationDirect : IAnimation
    //{
    //    public readonly Vector Dist;
    //    public MovementAnimationDirect(Vector dist)
    //    {
    //        Dist = dist;
    //    }

    //    public Vector2 GetDisposition(float tickFrac)
    //    {
    //        return -Dist.ToFloat() * ViewHandler.GRID_SIZE * tickFrac;
    //    }
    //}

    /// <summary>
    /// A movement animation consisting of potentially multiple small segments posited after one another.
    /// </summary>
    public class MovementAnimation : IAnimation
    {
        public readonly List<Vector> Steps;
        public MovementAnimation(List<Vector> steps)
        {
            Steps = steps;
            Steps.Reverse(); // Steps have to be reversed as they are being offset FROM the ending position!
        }

        public MovementAnimation(Vector step)
        {
            Steps = new List<Vector>() { step };
        }

        public Vector2 GetDisposition(float tickFrac)
        {
            var offset = Vector2.Zero;
            var stepTime = 1f / Steps.Count;
            var frac = tickFrac;

            foreach (var step in Steps)
            {
                if (tickFrac > stepTime) // >= or > ? i.e. should tickFrac == 0, or tickFrac == 1 be incorporated?
                {
                    offset += step.ToFloat() * ViewHandler.GRID_SIZE;
                    tickFrac -= stepTime;
                }
                else
                {
                    offset += step.ToFloat() * ViewHandler.GRID_SIZE * tickFrac / stepTime;
                    break;
                }
            }
            return -offset;
        }
    }
}
