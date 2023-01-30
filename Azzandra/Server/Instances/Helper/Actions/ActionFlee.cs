using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionFlee : EntityAction
    {
        public readonly Instance Inst;
        
        public ActionFlee(Entity caller, Instance inst) : base(caller)
        {
            Inst = inst;
        }

        protected override bool PerformAction()
        {
            // Get negative distance and boundarize by movement speed for each axis.
            var dist = (Caller.Position - Inst.Position);
            dist = Vector.Smallest(dist, dist.Sign() * Caller.GetMovementSpeed());

            if (dist == Vector.Zero)
                dist = Dir.Random.ToVector();

            return new ActionMove(Caller, dist, false).Perform();
        }
    }
}
