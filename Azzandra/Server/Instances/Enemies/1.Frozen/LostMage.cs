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

        public override int AggressiveRange => 5;
        public override bool CanOpenDoors() => true;


        public LostMage(int x, int y) : base(x, y) { }


        protected override void SetupActionPotentials()
        {
            Spells.Add( new TemplateSpell(2, 6, 4, new SpellEffects.Disorient())
            {
                Requirement = c => !c.Target?.Entity?.HasStatusEffect(StatusEffectID.Disoriented) ?? false
            });
            Spells.Add( new TemplateSpell(2, 6, 4, new SpellEffects.Weaken())
            {
                Requirement = c => !c.Target?.Entity?.HasStatusEffect(StatusEffectID.Weak) ?? false
            });
        }

        public override Symbol GetSymbol() => new Symbol("L", Color.Lavender);
    }
}
