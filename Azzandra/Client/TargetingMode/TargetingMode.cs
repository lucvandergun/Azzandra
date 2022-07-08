using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.TargetingMode
{
    public abstract class TargetingMode
    {
        public EntityAction InboundAction { get; set; }
        
        public TargetingMode()
        {
            
        }

        public abstract void CheckSwitchTarget(InputHandler ih);
        public abstract void CheckPerformAction(InputHandler ih);
        public abstract void PerformTargetAction(InputHandler ih);
        public abstract bool HasTarget(Server server);
        public abstract string GetActionString(Server server);
    }
}
