using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class ActionVector : EntityAction
    {
        public Vector Target;

        public ActionVector(Entity caller, Vector target) : base(caller)
        {
            Target = target;
        }
    }
}
