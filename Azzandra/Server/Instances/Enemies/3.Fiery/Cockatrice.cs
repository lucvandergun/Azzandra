using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Cockatrice : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;
        public override int GetW() => 1;
        public override int GetH() => 1;

        private Vector? LockedPosition;


        public Cockatrice(int x, int y) : base(x, y)
        { }

        public override void Turn()
        {
            if (Target == null)
            {
                LockedPosition = null;
            }
            base.Turn();
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

            // If has a locked position:
            if (LockedPosition != null)
            {
                if (CanSee(LockedPosition.Value))
                {
                    if (AttackTimer < 3)
                        return null;

                    var pos = LockedPosition.Value;
                    LockedPosition = null;

                    if (!target.IsInRegion(new Region(pos, new Vector(1, 1)))) // - Vector.One
                    {
                        target.Level.Server.User.ShowMessage((target is Player ? "You have" : target.ToStringAdress().CapFirst() + " has") + " avoided " + ToStringAdress() + "'s gaze!");
                        return null;
                    }

                    target.Level.Server.User.ShowMessage("<red>" + ToStringAdress().CapFirst() + " turns " + (target is Player ? "you" : target.ToStringAdress()) + " to stone!");
                    target.AddStatusEffect(new StatusEffects.Stunned(1, 10), true);

                    var attack = new DirectDamage(Level.Server, Style.Other, 40, false);
                    return new ActionAffect(this, target, attack);
                }
                else
                {
                    LockedPosition = null;
                }
            }

            // Set locked pos if target visible and target is not already stunned.
            if (LockedPosition == null && DistanceTo(target).ChebyshevLength() <= 4 && CanSee(target) && !target.HasStatusEffect(StatusEffectID.Stunned))
            {
                if (AttackTimer >= 10)
                {
                    LockedPosition = target.Position;
                    AttackTimer = 0;
                    target.Level.Server.User.ShowMessage("<red>" + ToStringAdress().CapFirst() + " gazes onto " + (target is Player ? "your" : target.ToStringAdress() + "'s") + " location!");
                }
                return null;
            }

            // If not next to target - move towards:
            return new ActionMoveTo(this, target);
        }


        public override Symbol GetSymbol() => new Symbol('c', Color.DarkKhaki);


        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            int x = BitConverter.ToInt32(bytes, pos);
            int y = BitConverter.ToInt32(bytes, pos);
            if (x != -1 || y != -1)
                LockedPosition = new Vector(x, y);
            pos += 8;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[8];
            bytes.Insert(0, BitConverter.GetBytes(LockedPosition == null ? -1 : LockedPosition.Value.X));
            bytes.Insert(4, BitConverter.GetBytes(LockedPosition == null ? -1 : LockedPosition.Value.Y));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
