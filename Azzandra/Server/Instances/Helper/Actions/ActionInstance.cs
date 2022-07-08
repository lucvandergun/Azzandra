using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionInstance : EntityAction
    {
        public readonly Instance Target;
        public readonly bool IsShift;

        public ActionInstance(Entity caller, Instance target, bool isShift = false) : base(caller)
        {
            Target = target;
            IsShift = isShift;
        }

        protected override bool PerformAction()
        {
            if (!(Caller is Player player))
                return false;
            var user = player.User;
            
            var inst = Target;
            if (inst.IsInteractable())
            {
                if (inst.IsInInteractionRange(Caller))
                {
                    inst.Interact(Caller);
                    return true;
                }
                else
                {
                    user.ShowMessage("You can't reach that!");
                    return false;
                }
            }
            else if (inst.IsAttackable())
            {
                var style = user.Equipment.AttackStyle;
                var attack = new Attack(
                    user.Server,
                    style,
                    user.Equipment.AttackSpeed,
                    user.Equipment.AttackRange,
                    player.GetAcc(style),
                    player.GetDmg(style),
                    user.Equipment.GetAttackProperties());

                // If cannot (yet) attack, and style is melee
                if (IsShift && user.Equipment.AttackStyle == Style.Melee && Caller.TileDistanceTo(Target) <= 1 + user.Equipment.AttackRange && !Caller.CanAffect(inst, attack))
                {
                    if (!new ActionMoveTo(Caller, Target).Perform())
                        return false;
                }

                if (Caller.CanAffect(inst, attack))
                {
                    // Check whether caller has waited long enough to attack with its current weapon
                    if (player.AttackTimer < player.GetAttackSpeed(style))
                    {
                        // Keep attempting until succesful attack if 'ReQueueing' setting = true
                        if (user.Server.ReQueueing)
                            Caller.NextAction = new ActionAffect(Caller, inst, attack);
                        return true;
                    }
                    else
                    {
                        Caller.Affect(inst, attack);
                        return true;
                    }
                }
            }

            return true;
        }
    }
}
