using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Skeleton : Enemy
    {
        public override EntityType EntityType => EntityType.Skeleton;
        public bool IsActive { get; protected set; } = true;
        public override bool IsAttackable() => IsActive;
        public override bool IsSolid() => IsActive;
        public override bool CanFlee() => false;

        public override string AssetName => IsActive ? base.AssetName : (base.AssetName + "_bones");


        public Skeleton(int x, int y) : base(x, y) { }

        public override void Turn()
        {
            if (IsActive)
            {
                base.Turn();
                return;
            }

            if (Target != null)
            {
                if (!CanSee(Target.Instance) || TileDistanceTo(Target.Instance) >= 5)
                {
                    IsActive = true;
                    UpdateAnimation();
                }
            }
            else
            {
                IsActive = true;
                UpdateAnimation();
            }
        }

        public override int GetHit(Style style, int dmg)
        {
            var hit = base.GetHit(style, dmg);
            if (Hp <= FullHp / 3 && Hp > 0)
            {
                IsActive = false;
                UpdateAnimation();
                Level.Server.User.ShowMessage("<gray>" + ToStringAdress().CapFirst() + " turns into a pile of bones.");
            }
            return hit;
        }


        public override Symbol GetSymbol() => new Symbol('s', Color.White);

        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Active
            IsActive = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[1];
            bytes.Insert(0, BitConverter.GetBytes(IsActive));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
