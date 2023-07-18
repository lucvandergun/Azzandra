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
        public int Level = 1;

        public TemplateSpell()
        { }

        public TemplateSpell(int speed, int range, int spc, SpellEffect effect, int level = 1, List<AttackProperty> properties = null) : base(speed, range)
        {
            SpellEffect = effect;
            Spellcast = spc;
            Level = level;
        }

        public override Affect ToAffect(Server server)
        {
            var spell = new Spell(server, Speed, Range, Spellcast, SpellEffect, Level, Properties);
            return spell;
        }

        public override string ToString() => SpellEffect.GetType().Name;
    }
}
