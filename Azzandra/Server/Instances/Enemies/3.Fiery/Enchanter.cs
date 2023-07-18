using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Enchanter : Enemy
    {
        public override EntityType EntityType => EntityType.Humanoid;
        public override bool CanOpenDoors() => true;
        public override int AggressiveRange => 5;

        public Enchanter(int x, int y) : base(x, y) { }

        protected override void SetupActionPotentials()
        {
            Spells.Add(new TemplateSpell(3, 4, 7, new SpellEffects.WindBlast())
            {
                Requirement = c => !c.Target?.Combatant?.HasStatusEffect(StatusEffectID.Stunned) ?? false
            });
            Spells.Add(new TemplateSpell(3, 4, 7, new SpellEffects.Freeze())
            {
                Requirement = c => !c.Target?.Combatant?.HasStatusEffect(StatusEffectID.Frozen) ?? false
            });
            Spells.Add(new TemplateSpellAcute(new SpellEffects.Deflect())
            {
                Requirement = c => !c.HasStatusEffect(StatusEffectID.Deflect)
            });
        }

        public override Symbol GetSymbol() => new Symbol('E', Color.MediumAquamarine); //LightSeaGreen
    }
}
