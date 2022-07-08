using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.TargetingMode
{
    public class InstanceTargeting : TargetingMode
    {
        public readonly bool CanTargetPlayer;

        public override bool HasTarget(Server server) => server.User.Target != null;
        public override string GetActionString(Server server) => InboundAction is ActionAffect a && a.Affect is Spell ? "cast"
            : server.User.Target.IsAttackable() ? "attack"
            : server.User.Target.IsInteractable() ? server.User.Target is GroundItem ? "pick-up" : "interact"
            : "-";

        public InstanceTargeting(bool canTargetPlayer = false)
        {
            CanTargetPlayer = canTargetPlayer;
        }
        
        public override void CheckSwitchTarget(InputHandler ih)
        {
            if (Input.IsKeyPressed[Keys.T])
            {
                SwitchTarget(ih);
            }
        }

        public virtual void SwitchTarget(InputHandler ih)
        {
            var target = ih.Server.User.Target;

            var potentialTargets = GetPotentialTargets(ih.Server);


            // Pick a target: if no target ? pick the first else pick the 'next'
            if (target == null)
            {
                if (potentialTargets.Count >= 1)
                    ih.Server.User.SetTarget(potentialTargets[0]);
                else
                    ih.Server.User.Log.Add("You don't see any instances to target.");
            }
            else
            {
                // Get current instance number based on distance to player
                var instanceNumber = potentialTargets.IndexOf(target);

                // Move to next: add 1 to current target instance's index, loop back to start if it is at the end
                var newInstanceNumber = instanceNumber + 1;

                if (newInstanceNumber < potentialTargets.Count)
                    ih.Server.User.SetTarget(potentialTargets[newInstanceNumber]);
                else
                    ih.Server.User.SetTarget(null);
            }
        }

        protected virtual List<Instance> GetPotentialTargets(Server server)
        {
            var list = server.User.VisibilityHandler.VisibleInstances.CreateCopy().Where(i => i.CanBeTargetedByPlayer()).ToList();
            if (!CanTargetPlayer) list.RemoveAll(i => i is Player);
            return list;
        }

        public override void CheckPerformAction(InputHandler ih)
        {
            // Set attack command
            if (Input.IsKeyPressed[Keys.Space] || ih.Server.TurnDelay == 0 && Input.IsKeyDown[Keys.Space])
            {
                PerformTargetAction(ih);
                return;
            }

            // Set directional command
            if (ih.Dir != null && (ih.Server.IsPlayerTurn || ih.CanInit))
            {
                ih.Server.SetPlayerAction(new ActionDirectional(ih.Server.User.Player, ih.Dir, ih.CanInit, ih.IsShift));

                return;
            }
        }

        public override void PerformTargetAction(InputHandler ih)
        {
            var target = ih.Server.User.Target;
            if (target != null && ih.Server.LevelManager.CurrentLevel.CheckInstanceExists(target))
            {
                PerformAction(ih, target);
            }
            else
            {
                ih.Server.User.Log.Add("<rose>You don't have a target selected.");
            }
        }

        public void PerformAction(InputHandler ih, Instance target)
        {
            if (InboundAction is ActionAffect a)
            {
                // Check for affectable: // && ih.Server.User.Player.Sp < Data.GetSpellData(spell.SpellEffect).SpellPoints
                if (a.Affect is Spell spell)
                {
                    //ih.Server.User.Player.User.ShowMessage("<rose>You don't have enough spell points to cast that spell!");
                    if (!ih.Server.User.Player.CanAffect(target, spell))
                        return;
                }

                a.Target = target;
                ih.Server.SetPlayerAction(a);
                ih.TargetingMode = ih.DefaultTargetingMode;
                return;
            }

            ih.Server.SetPlayerAction(new ActionInstance(ih.Server.User.Player, target, ih.IsShift));
        }

        public override string ToString()
        {
            var iastr = InboundAction == null ? "" : " [" + InboundAction.ToString() + "]";
            return GetType().Name + iastr;
        }
    }
}
