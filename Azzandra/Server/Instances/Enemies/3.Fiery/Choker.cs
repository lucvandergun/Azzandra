using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Choker : Enemy
    {
        public override EntityType EntityType => EntityType.Demon;
        public override int GetW() => 2;
        public override int GetH() => 2;

        private int ConsecutiveStrangles = 0;
        public override bool CanFlee() => false;

        public Choker(int x, int y) : base(x, y)
        { }

        public override void TurnStart()
        {
            base.TurnStart();

            if (AttackTimer >= 3)
                ConsecutiveStrangles = 0;
        }

        public override EntityAction DetermineAggressiveAction()
        {
            // Just to make sure: check whether target actually exists
            var target = Target.Entity;
            if (target == null)
            {
                Target = null;
                return null;
            }

            // If next to target - try to strangle:
            if (IsTouching(target) && ConsecutiveStrangles < 3)
            {
                if (ConsecutiveStrangles <= 0)
                    target.Level.Server.User.ShowMessage("<red>" + ToStringAdress().CapFirst() + " starts to strangle " + (target is Player ? "you" : target.ToStringAdress()) + "!");
                
                var attack = new DirectDamage(Level.Server, Style.Melee, 15, true);
                ConsecutiveStrangles++;

                target.Action = new ActionMove(target, Vector.Zero, true);
                return new ActionAffect(this, target, attack);
            }

            // If not next to target - move towards:
            return new ActionMoveTo(this, target);
        }


        public override Symbol GetSymbol() => new Symbol('c', Color.PaleVioletRed);

        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            ConsecutiveStrangles = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(ConsecutiveStrangles);

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
