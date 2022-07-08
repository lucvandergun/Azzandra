using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class LostMage : Enemy
    {
        public override EntityType EntityType => EntityType.Humanoid;


        public LostMage(int x, int y) : base(x, y) { }


        protected override void SetupActionPotentials()
        {
            base.SetupActionPotentials();

            ActionPotentials.Add( new TemplateSpell(2, 6, 4, new SpellEffects.Disorient())
            {
                Requirement = c => !c.Target?.Combatant?.HasStatusEffect(StatusEffectID.Disoriented) ?? false
            });
            ActionPotentials.Add( new TemplateSpell(2, 6, 4, new SpellEffects.Weaken())
            {
                Requirement = c => !c.Target?.Combatant?.HasStatusEffect(StatusEffectID.Weak) ?? false
            });
        }

        public override Symbol GetSymbol() => new Symbol("L", Color.Lavender);
    }
}
