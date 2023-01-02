using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.TargetingMode
{
    public class EntityTargeting : InstanceTargeting
    {
        public EntityTargeting(bool canTargetPlayer = false) : base(canTargetPlayer)
        {

        }
        
        protected override List<Instance> GetPotentialTargets(Server server)
        {
            if (!server.GameClient.InputHandler.IsShift)
                return base.GetPotentialTargets(server).Where(i => i is Entity).ToList();

            return base.GetPotentialTargets(server);
        }
    }
}
