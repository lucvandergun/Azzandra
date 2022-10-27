using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Wraith : Enemy
    {
        public override EntityType EntityType => EntityType.Spirit;
        public bool IsAngered { get; set; } = false;

        protected override bool IsAgressive() => IsAngered;
        public override string Name => IsAngered ? "angered wraith" : "wraith";
        public override bool IsSolid() => false;


        public Wraith(int x, int y) : base(x, y) { }

        public override void Turn()
        {
            // Check whether wraith gets angry: if potential target is in 3x3 area surrounding it
            if (!IsAngered)
            {
                var creatures =
                    Level.ActiveInstances
                        .Where(i => i is Entity && IsInRange(i, 1)
                        //.Where(i => i.GetTiles()
                        //.Intersect(Vector.Dirs9.ConvertAll(t => t + Position)).Count() > 0
                    );
                foreach (var inst in creatures)
                {
                    if (IsATarget((Entity)inst))
                    {
                        if (inst is Player player)
                        {
                            if (player.User.Equipment.HasItem(i => i is Items.Weapon))
                            {
                                IsAngered = true;
                                Target = new InstRef(player);
                                player.User.ShowMessage("<medblue>The wraith gets angered as you display your offensive intentions towards it!");
                            }
                        }
                    }
                }
            }

            base.Turn();
        }

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            // Set angered if attacked
            if (!IsAngered && Target == null)
            {
                IsAngered = true;
                var str = attacker is Player ? "you display your" : attacker.ToStringAdress() + " displays its";
                Level.Server.User.ShowMessage("<medblue>The wraith gets angered as " +str+ " offensive intentions towards it!");
            }

            return base.GetAffected(attacker, affect);
        }


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Active
            IsAngered = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[1];
            bytes.Insert(0, BitConverter.GetBytes(IsAngered));

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        public override Symbol GetSymbol() => new Symbol("W", Color.White);
    }
}
