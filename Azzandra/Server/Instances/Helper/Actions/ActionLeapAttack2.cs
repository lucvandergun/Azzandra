using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionLeapAttack2 : ActionPathTarget
    {
        public Attack Attack { get; private set; }
        
        public ActionLeapAttack2(Entity caller, Entity target, Attack attack) : base(caller, target, false)
        {
            Target = target;
            Attack = attack;
        }
        
        protected override bool PerformAction()
        {
            // Firstly: try to move towards the target:
            var moved = base.PerformAction();

            // Secondly: attack the target
            if (Caller.CanAffect(Target, Attack))
            {
                if (Caller.AttackTimer < Attack.Speed)
                {
                    if (!(Caller is Player) || Caller.Level.Server.GameClient.Engine.Settings.ReQueueing)
                        Caller.NextAction = this;
                    return true;
                }

                Attack = (Attack)Caller.Affect(Target, Attack);
                Caller.AttackTimer = -1; // Leap attacks introduce additional time to recover for one's next attack.
            }
                
            return moved;
        }
    }
}
