using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TemplateSpell : TemplateAffect
    {
        public SpellEffect SpellEffect;
        public int Spellcast = 1;
        public List<AttackProperty> Properties;

        public TemplateSpell()
        { }

        public TemplateSpell(int speed, int range, int spc, SpellEffect effect, List<AttackProperty> properties = null) : base(speed, range)
        {
            SpellEffect = effect;
            Spellcast = spc;
        }

        public override Affect ToAffect(Server server)
        {
            var spell = new Spell(server, Speed, Range, Spellcast, SpellEffect, Properties);
            return spell;
        }

        public override string ToString() => SpellEffect.GetType().Name;
    }
}
