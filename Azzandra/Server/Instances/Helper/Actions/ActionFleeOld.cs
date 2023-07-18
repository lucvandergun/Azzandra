using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionFleeOld : EntityAction
    {
        public readonly List<Instance> Threats;

        public ActionFleeOld(Entity caller, Instance threat) : base(caller)
        {
            Threats = new List<Instance>() { threat };
        }
        public ActionFleeOld(Entity caller, List<Instance> threats) : base(caller)
        {
            Threats = threats;
        }

        protected override bool PerformAction()
        {
            // Get negative distance and boundarize by movement speed for each axis.
            var inst = Threats[0];
            var dist = (Caller.Position - inst.Position);
            dist = Vector.Smallest(dist, dist.Sign() * Caller.GetMovementSpeed());

            if (dist == Vector.Zero)
                dist = Dir.Random.ToVector();

            return new ActionMove(Caller, dist, false).Perform();
        }
    }
}
