using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionAffect : EntityAction
    {
        public Instance Target;
        public Affect Affect;

        public ActionAffect(Entity caller, Instance target, Affect affect) : base(caller)
        {
            Target = target;
            Affect = affect;
        }
        
        protected override bool PerformAction()
        {
            // Re-Queue the action if desired and on attack timer
            if (Caller.AttackTimer < Affect.Speed)
            {
                if (!(Caller is Player p) || p.ReQueueActions()) //Caller.Level.Server.GameClient.Engine.Settings.ReQueueing
                    Caller.NextAction = this;
                return true;
            }

            // Remove player's spellpoints
            if (Caller is Player player && Affect is Spell spell)
            {
                var data = Data.GetSpellData(spell.SpellEffect);
                player.Sp -= data.SpellPoints;
            }

            Affect = Caller.Affect(Target, Affect);
            return true;
        }
    }
}
