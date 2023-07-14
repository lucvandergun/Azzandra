using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Ghast : Enemy
    {
        public override EntityType EntityType => EntityType.Undead;
        public override int GetMoveDelay() => 3;
        public override bool CanFlee() => false;
        public override int AggressiveRange => 5;

        public Ghast(int x, int y) : base(x, y) { }

        public override EntityAction DetermineAggressiveAction()
        {
            // Just to make sure: check whether target actually exists
            var target = Target.Combatant;
            if (target == null)
            {
                Target = null;
                return null;
            }

            if (CanSee(target) && DistanceTo(target).ChebyshevLength() <= 2 && AttackTimer >= 3)
            {
                MoveTimer = 3;
                AttackTimer = 0;
                return new ActionSpellAcute(this, new SpellEffects.Scream());
            }
            
            if (MoveTimer == 0)
            {
                AttackTimer = 0;
                MoveTimer = GetMoveDelay();

                // Move as much of path as possible:
                var path = new Path(this, target.Position, false);
                var movements = new List<Vector>();
                while (true)
                {
                    var step = path.GetNextStep();
                    if (step == null) break;
                    var movement = Move(step.Value);
                    if (movement.Count <= 0)
                        break;
                    else
                        movements.AddRange(movement);
                }

                Animations.Add(new MovementAnimation(this, movements, GetInitiative()));
            }

            return null; 
        }

        public override Symbol GetSymbol() => new Symbol('G', Color.CadetBlue);
    }
}
