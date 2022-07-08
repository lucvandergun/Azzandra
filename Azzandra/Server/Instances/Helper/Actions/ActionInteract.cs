using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionInteract : EntityAction
    {
        public readonly Instance Target;

        public ActionInteract(Entity caller, Instance target) : base(caller)
        {
            Target = target;
        }

        protected override bool PerformAction()
        {
            Target.Interact(Caller);
            return true;
        }
    }
}
