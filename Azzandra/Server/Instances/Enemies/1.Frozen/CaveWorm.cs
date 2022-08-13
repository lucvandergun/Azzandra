using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CaveWorm : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;
        public override bool CanMoveDiagonal() => false;
        public override bool IsInstanceSolidToThis(Instance inst)
        {
            return inst is CaveWormTail ?
                // return cwt is not a child
                !Util.RecursiveValidator(
                    Children.FirstOrDefault(c => c.Instance is CaveWormTail)?.Instance,
                    (cwt) => cwt.Children.FirstOrDefault(cwt2 => cwt2.Instance is CaveWormTail)?.Instance,
                    (cwt3) => cwt3 == inst
                )
                : base.IsInstanceSolidToThis(inst);
        }

        public CaveWorm(int x, int y) : base(x, y) {}

        public override void Init()
        {
            // Create all the tail parts once and only:
            var tail = new CaveWormTail(X, Y, this, 3);
            Level.CreateInstance(tail);
            Children.Add(tail.CreateRef());
        }

        public override Symbol GetSymbol() => new Symbol('c', Color.AntiqueWhite);


        public override List<Vector> Move(Vector distance, bool orthoDiagonal = true, bool hasSlided = false)
        {
            var oldPos = Position;
            var steps = base.Move(distance, hasSlided);
            if (steps.Count > 0)
            {
                var tail = Children.FirstOrDefault(r => r.Instance is CaveWormTail).Instance;
                if (tail != null)
                {
                    tail.Move(oldPos - tail.Position);
                }
            }
            return steps;
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
