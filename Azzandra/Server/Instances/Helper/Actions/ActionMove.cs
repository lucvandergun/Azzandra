using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionMove : EntityAction
    {
        public Vector Dist { get; private set; }
        public bool HasSlided = false;
        
        public ActionMove(Entity caller, Vector dist, bool isForced = false) : base(caller)
        {
            Dist = dist;
            IsForced = isForced;
        }

        protected override bool PerformAction()
        {
            // Delay action if move timer is still on delay:
            if (Caller.MoveTimer > 0 && !IsForced)
            {
                if (!(Caller is Player player) || player.ReQueueActions())
                    Caller.NextAction = this;
                return true;
            }

            // Return if caller can't move (due to status effects) and the action is unforced.
            if (!IsForced && !Caller.CanMove())
                return false;

            // Disorient distance if affected by status effect:
            if (Caller.HasStatusEffect(StatusEffectID.Disoriented))
                Dist = Dir.Random.ToVector();

            // Move specified distance:
            var movement = Caller.Move(Dist, true, HasSlided);

            // Set movetimer if moved some distance at all:
            if (movement.Exists(s => !s.IsNull()))
                Caller.MoveTimer = Caller.GetMoveDelay();

            return true; // Return whether moved?
        }

        public override string ToString()
        {
            return "Move: " + Dist + ", hasSlided: " + HasSlided;
        }
    }
}
