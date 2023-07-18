using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class VineBlight : Enemy
    {
        public override EntityType EntityType => EntityType.Plant;

        public override int GetW() => 2;
        public override int GetH() => 2;
        public override bool CanFlee() => false;

        public bool HasVine() => Children.Any(c => c.Instance is Vine vine);

        public VineBlight(int x, int y) : base(x, y) { }

        public override void TurnStart()
        {
            // Reset attack timer if loses vine.
            var hadVine = HasVine();
            base.TurnStart();

            if (Target == null)
                Children.FilterOut(c => c.Instance is Vine vine);

            if (hadVine && !HasVine())
                AttackTimer = 0;
        }

        public override EntityAction DetermineAggressiveAction()
        {
            // Just to make sure: check whether target actually exists
            var target = Target.Combatant;
            if (target == null)
            {
                Target = null;
                return null;
            }

            // If vined, keep vining, else capture target.
            if (HasVine())
            {
                var attack = new DirectDamage(Level.Server, Style.Melee, 4, true);
                Affect(target, attack);
            }
            
            var affect = new Spell(Level.Server, 4, 4, 4, new SpellEffects.Entangle());
            var action = CreateActionForAffect(target, affect);
            return action;
        }

        public override Symbol GetSymbol() => new Symbol('V', Color.SeaGreen);
    }
}
