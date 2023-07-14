using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Gargoyle : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Rock;
        public bool IsActive { get; protected set; } = false;
        public override int DetectRange => 2;


        public override string AssetName => IsActive ? "gargoyle_flying" : "gargoyle_idle";
        public override Color AssetLightness => !IsActive ? Color.DarkGray : Color.White;


        public Gargoyle(int x, int y) : base(x, y) { }

        public override EntityAction DetermineRegularAction()
        {
            // Perform any present actions:
            if (Action is ActionPath)
                return Action;

            // Path to base position
            if (BasePosition != null && Position != BasePosition)
            {
                return new ActionPath(this, BasePosition.Value, true, false);
            }
            else
            {
                IsActive = false;
                SetMoveType(MoveType.Walk);
                AnimationManager.Play("gargoyle_idle");
                return null;
            }
        }

        protected override Entity FindTarget()
        {
            var target =  base.FindTarget();
            if (target != null)
            {
                IsActive = true;
                SetMoveType(MoveType.Fly);
                AnimationManager.Play("gargoyle_flying");
            }

            return target;
        }

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            IsActive = true;
            SetMoveType(MoveType.Fly);
            AnimationManager.Play("gargoyle_flying");

            return base.GetAffected(attacker, affect);
        }


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Active
            IsActive = BitConverter.ToBoolean(bytes, pos);
            pos += 1;
            SetMoveType(IsActive ? MoveType.Fly : MoveType.Walk);

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[1];
            bytes.Insert(0, BitConverter.GetBytes(IsActive));

            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override Symbol GetSymbol() => new Symbol('G', IsActive ? Color.Gray : new Color(63, 63, 63));
    }
}
