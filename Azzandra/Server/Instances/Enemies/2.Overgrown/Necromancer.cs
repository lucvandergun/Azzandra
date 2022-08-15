using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Necromancer : Enemy
    {
        public override EntityType EntityType => EntityType.Humanoid;


        public Necromancer(int x, int y) : base(x, y) { }


        protected override void SetupActionPotentials()
        {
            Spells.Add(
                new TemplateSpell(2, 6, 10, new SpellEffects.Revive())
                {
                    Requirement = new Func<Entity, bool>(c => c.Level.ActiveInstances.Where(a => a is Grave g && !g.IsEmpty).Any(a => c.CanSee(a))),
                    TargetSelector = new Func<Entity, Instance>(c =>
                        c.Level.ActiveInstances.Where(a => a is Grave g && !g.IsEmpty).FirstOrDefault(a => c.CanSee(a))
                    )
                });
        }

        public override Symbol GetSymbol() => new Symbol("N", Color.Purple);
    }
}
