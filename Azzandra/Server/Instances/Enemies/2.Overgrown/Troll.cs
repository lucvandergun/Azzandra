using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Troll : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;

        public override int GetW() => 2;
        public override int GetH() => 2;
        public override int LoseTargetTime => -1;
        private int RegenTimer = 0;
        private const int RegenDelay = 2;

        public Troll(int x, int y) : base(x, y) { }

        public override void TickStart()
        {
            base.TickStart();

            // Regeneration
            if (RegenTimer == 0 && Hp > 0 && !HasStatusEffect(StatusEffectID.Burning))
            {
                Heal(1);
                RegenTimer = RegenDelay;
            }

            if (RegenTimer > 0) RegenTimer--;
            
        }

        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            RegenTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            base.Load(bytes, ref pos);
        }
        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(RegenTimer);
            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override Symbol GetSymbol() => new Symbol('T', Color.DarkGray);
    }
}
