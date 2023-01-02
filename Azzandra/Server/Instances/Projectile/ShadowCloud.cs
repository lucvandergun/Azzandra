using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ShadowCloud : Entity
    {
        public override EntityType EntityType => EntityType.NonPhysical;
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override int GetMovementSpeed() => 2;
        public override Symbol GetSymbol() => new Symbol('@', Color.SlateBlue);
        public override bool IsSolid() => false;
        public override bool IsInstanceSolidToThis(Instance inst) => false;
        public override MoveType GetMovementType() => MoveType.Fly;

        protected InstRef Target;
        public int Time = 30;
        public readonly int PathMoveDelay = 5;
        public int PathMoveTimer = 0;

        public ShadowCloud(int x, int y, Entity origin, Entity target) : base(x, y)
        {
            // Set target instance id:
            Target = new InstRef(target);
        }

        public ShadowCloud(int x, int y) : base(x, y)
        { }

        public override void TurnStart()
        {
            base.TurnStart();

            Time--;
            if (Time <= 0)
                Destroy();

            if (PathMoveTimer > 0) PathMoveTimer--;

            // Check target and origin still exists..
            if (!Target?.Exists() ?? true)
            {
                Destroy();
            }
        }

        public override void Turn()
        {
            base.Turn();

            // Action present? Keep it. Else make new path to target.
            if (Action != null)
            {
                PathMoveTimer = PathMoveDelay;
                return;
            }

            if (PathMoveTimer > 0)
                return;

            var target = Target?.Combatant;
            if (target != null)
            {
                Action = new ActionPath(this, target.Position, false);
            }
        }

        public override void OnCollisionWithInstance(Instance inst)
        {
            if (!(inst is Entity entity))
                return;
            
            if (entity is Azzandra)
                return;

            if (entity.AddStatusEffect(new StatusEffects.Blind(2, 8), true) && entity is Player player)
                player.User.ShowMessage("<red>You have been blinded by the shadow cloud!");
        }

        public override bool IsImmuneToStatusEffect(int statusID, string name) => true;
        public override bool IsAttackable() => false;


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Target ID
            Target = new InstRef(BitConverter.ToInt32(bytes, pos));
            pos += 4;

            // Move timer
            PathMoveTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[8];

            // Target ID
            bytes.Insert(0, BitConverter.GetBytes(InstRef.GetSaveID(Target)));

            // Move timer
            bytes.Insert(4, BitConverter.GetBytes(PathMoveTimer));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
