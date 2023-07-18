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
        public int Level;

        public ActionVectorSpell(Entity caller, Vector target, SpellEffectVector effect, int level = 1) : base(caller, target)
        {
            SpellEffect = effect;
            Level = level;
        }

        protected override bool PerformAction()
        {
            // Remove player's spellpoints
            if (Caller is Player player)
            {
                var data = Data.GetSpellData(SpellEffect);
                player.Sp -= data.SpellPoints;
            }

            SpellEffect.Apply(Caller, Target, Level);
            return true;
        }
    }
}
