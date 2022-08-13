using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionPathTarget : EntityAction
    {
        public Path2 Path { get; protected set; }
        public Instance Target;

        public ActionPathTarget(Entity caller, Instance target, bool mustReach) : base(caller)
        {
            Target = target;
            Path = new Path2(caller, target, mustReach);
        }

        /// <summary>
        /// Retrieve the next step along the path. End pathing action if: 
        ///  1. The the target has been reached.
        ///  2. There is no step to be done (should not happen).
        ///  3. The step cannot be performed due to obstructions.
        /// </summary>
        /// <returns></returns>
        protected override bool PerformAction()
        {
            if (Path.Length <= 0)
                return false;

            var step = Path.GetNextStep();
            if (step == null)
                return false;

            if (!Caller.CanMoveUnobstructed(step.Value.X, step.Value.Y))
                return false;

            var dist = step.Value;
            new ActionMove(Caller, dist, IsForced).Perform();

            if (Path.Length > 0) // Could also be 1, but will wait a tick after reaching end this way.
                Caller.NextAction = this;

            return true;
        }

        public override string ToString()
        {
            return "A-Star: " + Path.Target;
        }
    }
}
