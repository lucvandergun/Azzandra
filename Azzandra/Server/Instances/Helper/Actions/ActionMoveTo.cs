using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionMoveTo : EntityAction
    {
        public readonly Instance Inst;
        
        public ActionMoveTo(Entity caller, Instance inst) : base(caller)
        {
            Inst = inst;
        }

        protected override bool PerformAction()
        {
            // Get distance and boundarize by movement speed for each axis.
            var dist = Caller.DistanceTo(Inst);
            dist = Vector.Smallest(dist, dist.Sign() * Caller.GetMovementSpeed());

            return new ActionMove(Caller, dist, IsForced).Perform();
        }
    }
}
