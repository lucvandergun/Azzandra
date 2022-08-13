using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CaveWormTail : Instance
    {
        private int Amt;
        public override bool IsSolid() => true;
        public override bool IsAttackable() => true;
        public override int Initiative => 16;
        public CaveWormTail(int x, int y) : base(x, y) {}

        public CaveWormTail(int x, int y, Instance parent, int amt) : base(x, y)
        {
            Parent = new InstRef(parent);
            ActionPotential = parent.ActionPotential;
            Amt = amt;
        }

        public override void Init()
        {
            if (Amt > 1)
            {
                var tail = new CaveWormTail(X, Y, this, --Amt);
                Level.CreateInstance(tail);
                Children.Add(tail.CreateRef());
            }
        }


        public override Symbol GetSymbol() => new Symbol('=', Color.BurlyWood);
        public override string ToString() => "cave worm's tail";
        public override bool RenderLightness => true;

        public override void Turn()
        {
            return;
        }

        public override List<Vector> Move(Vector distance, bool orthoDiagonal, bool hasSlided = false)
        {
            var oldPos = Position;
            var steps = base.Move(distance, hasSlided);
            if (steps.Count > 0)
            {
                var tail = Children.FirstOrDefault(r => r.Instance is CaveWormTail)?.Instance;
                if (tail != null)
                {
                    tail.Move(oldPos - tail.Position);
                }
            }
            return steps;
        }

        public override bool CanWalkOverBlock(Block block)
        {
            return true;
        }

        public override bool IsInstanceSolidToThis(Instance inst)
        {
            return false;
        }

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            var parent = Parent.Instance;
            if (parent != null)
            {
                return parent.GetAffected(attacker, affect);
            }
            else
            {
                Destroy();
            }

            return null;
        }

        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();
            foreach (var c in Children)
                if (c.Instance is CaveWormTail bt)
                    bt.Destroy();
        }
    }
}
