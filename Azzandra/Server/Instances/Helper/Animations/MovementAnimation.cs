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
        public int Duration { get; set; }
        private Instance _owner;
        public Instance Owner => _owner;

        private int AnimationLength;
        private int MomentOfStart;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">The owner instance of the animation.</param>
        /// <param name="steps">The list of relative steps the movement is to display.</param>
        /// <param name="duration">Is usually the initiative of the instance, i.e. the amount of ticks it takes to perform a turn: thus one full turn-length.</param>
        public MovementAnimation(Instance owner, List<Vector> steps, int duration)
        {
            Steps = steps;
            Steps.Reverse(); // Steps have to be reversed as they are being offset FROM the ending position!
            Duration = duration * Server.TICK_SPEED;
            _owner = owner;

            MomentOfStart = Owner.Level.Server.AmtUpdates;
            AnimationLength = Duration;
        }

        public MovementAnimation(Vector step)
        {
            Steps = new List<Vector>() { step };
        }

        public void Update()
        {
            Duration--;
            if (Duration <= 0)
                Owner.Animations.Remove(this);
        }

        public Vector2 GetDisposition()
        {
            var offset = Vector2.Zero;              // The accumulative offset
            var animationFraction = Math.Max(0, 1f - ((float)Owner.Level.Server.AmtUpdates - MomentOfStart) / AnimationLength);
            var stepFraction = 1f / Steps.Count;    // The time in frames per movement step

            // Line up all steps (and their offsets) until the point in the current animationFraction
            foreach (var step in Steps)
            {
                // If the step can be fully displayed: do so
                if (animationFraction > stepFraction) // >= or > ? i.e. should tickFrac == 0, or tickFrac == 1 be incorporated?
                {
                    offset += step.ToFloat() * ViewHandler.GRID_SIZE;
                    animationFraction -= stepFraction;
                }
                //Else, add only a fraction of it
                else
                {
                    offset += step.ToFloat() * ViewHandler.GRID_SIZE * animationFraction / stepFraction;
                    break;
                }
            }
            return -offset;
        }
    }
}
