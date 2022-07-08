using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionLeapAttack : EntityAction
    {
        public readonly Entity Target;
        public Attack Attack;
        public Vector Dist;

        public ActionLeapAttack(Entity caller, Entity target, Vector dist, Attack attack) : base(caller)
        {
            Target = target;
            Attack = attack;
            Dist = dist;
        }
        
        protected override bool PerformAction()
        {
            new ActionMove(Caller, Dist, IsForced).Perform();

            // Secondly: affecting
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
                
            return true;
        }
    }
}
