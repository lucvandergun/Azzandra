using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionVectorSpell : ActionVector
    {
        public SpellEffectVector SpellEffect;

        public ActionVectorSpell(Entity caller, Vector target, SpellEffectVector effect) : base(caller, target)
        {
            SpellEffect = effect;
        }

        protected override bool PerformAction()
        {
            // Remove player's spellpoints
            if (Caller is Player player)
            {
                var data = Data.GetSpellData(SpellEffect);
                player.Sp -= data.SpellPoints;
            }

            SpellEffect.Apply(Caller, Target);
            return true;
        }
    }
}
