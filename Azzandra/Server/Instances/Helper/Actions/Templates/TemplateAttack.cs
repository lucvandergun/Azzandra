using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TemplateAttack : TemplateAffect
    {
        public Style Style = Style.Melee;
        public int Accuracy = 1, Damage = 1;
        public List<AttackProperty> Properties;
        public bool ShowHitMessage = true;

        public override bool IsInRange(int range) => Range >= (Style == Style.Melee ? range - 1 : range);

        public TemplateAttack()
        { }

        public TemplateAttack(Style style, int speed, int range, int acc, int dmg, List<AttackProperty> properties = null) : base(speed, range)
        {
            Style = style;
            Accuracy = acc;
            Damage = dmg;
            Properties = properties;
        }

        public override Affect ToAffect(Server server)
        {
            var attack = new Attack(server, Style, Speed, Range, Accuracy, Damage, Properties);
            attack.ShowHitMessage = ShowHitMessage;
            return attack;
        }

        public override string ToString() => Style + "Attack";
    }
}
