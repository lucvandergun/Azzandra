using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionSpellAcute : EntityAction
    {
        public SpellEffectAcute SpellEffect;

        public ActionSpellAcute(Entity caller, SpellEffectAcute effect) : base(caller)
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

            SpellEffect.Apply(Caller);
            return true;
        }
    }
}
