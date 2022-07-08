using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TemplateSpellAcute : ActionTemplate
    {
        public SpellEffectAcute SpellEffect;

        public TemplateSpellAcute() { }

        public TemplateSpellAcute(SpellEffectAcute effect)
        {
            SpellEffect = effect;
        }

        public override EntityAction ToAction(Enemy caller)
        {
            return new ActionSpellAcute(caller, SpellEffect);
        }

        public override string ToString() => "Spell: " + SpellEffect.ToString();
    }
}
