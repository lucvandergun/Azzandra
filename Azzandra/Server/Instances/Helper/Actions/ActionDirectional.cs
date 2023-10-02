using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionDirectional : EntityAction
    {
        public readonly Dir Dir;
        public readonly bool CanInit;
        public readonly bool IsShift;

        public ActionDirectional(Entity caller, Dir dir, bool canInit, bool isShift) : base(caller)
        {
            Dir = dir;
            CanInit = canInit;
            IsShift = isShift;
        }

        protected override bool PerformAction()
        {
            if (!(Caller is Player player))
                return false;
            var user = player.User;

            var reqSolidity = (!IsShift && !Dir.IsNull()) || (Dir.IsNull() && IsShift);

            // If can initiate interaction
            if (CanInit)
            {
                // 1. Check for instance at position
                var inst = Caller.Level.InstanceCheckPosition(Caller.X + Dir.X, Caller.Y + Dir.Y, Caller, reqSolidity);

                if (inst != null)
                {
                    if (inst.IsInteractable())
                    {
                        if (inst.IsInInteractionRange(Caller))
                            inst.Interact(Caller);
                        else
                            user.ShowMessage("You can't reach that!");

                        return true;
                    }
                    else if (inst.IsAttackable())
                    {
                        var style = user.Equipment.AttackStyle;
                        var attack = new Attack(
                            user.Server,
                            style,
                            user.Equipment.AttackSpeed,
                            user.Equipment.AttackRange,
                            player.GetAcc(style),
                            player.GetDmg(style),
                            user.Equipment.GetAttackProperties());

                        if (Caller.CanAffect(inst, attack))
                        {
                            //new ActionAffect(Caller, inst, attack).Perform();

                            // Check whether caller has waited long enough to attack with its current weapon
                            if (player.AttackTimer < player.GetAttackSpeed(style))
                            {
                                // Keep attempting until succesful attack if 'ReQueueing' setting = true
                                if (user.Server.ReQueueing)
                                    Caller.NextAction = new ActionAffect(Caller, inst, attack);
                                return true;
                            }
                            else
                            {
                                Caller.Affect(inst, attack);
                                return true;
                            }
                        }
                    }
                }

                // 2. Check for tile at position
                var blockPos = Caller.Level.BlockCheckPosition(player.Position + Dir.ToVector(), reqSolidity);
                if (blockPos != null)
                {
                    var block = Caller.Level.GetBlock(blockPos.Value);
                    if (block.Data.IsInteractable())
                    {
                        block.Data.OnInteraction(Caller.Level, blockPos.Value, player);
                        return true;
                    }
                }
                // 2.2 Check for non-solid cobwebs when shifting and dir is (0, 0):
                if (Dir.IsNull() && IsShift && Caller.Level.GetBlock(new BlockPos(player.Position, false)).ID == BlockID.Cobweb)
                {
                    Caller.Level.GetBlock(new BlockPos(player.Position, false)).Data.OnInteraction(Caller.Level, new BlockPos(player.Position, false), player);
                    return true;
                }
            }

            // If caller can't move at all: e.g. stunned or frozen, return true to pass the turn.
            if (!Caller.CanMove())
                return true;

            // 3. Move in direction:

            // Determine whether player can move unobstructed
            var unobstructed = Caller.CanMoveUnobstructed(Dir.X, Dir.Y);

            if (Caller.Level.Server.GameClient.Engine.Settings.SlidingDiagonals)
            {
                // Orthoganize direction if diagonal is not possible
                if (!unobstructed && Dir.IsDiagonal())
                {
                    unobstructed = Caller.CanMoveUnobstructed(Dir.X, 0) || Caller.CanMoveUnobstructed(0, Dir.Y);
                }
            }


            // Move if can move:
            if (unobstructed || Caller.HasStatusEffect(StatusEffectID.Disoriented))
            {
                new ActionMove(Caller, Dir.ToVector() * Caller.GetMovementSpeed()).Perform();
                // return true;
            }
            else return false;

            // Attempt finishing movement by an attack: LeapAttack
            if (!Dir.IsNull() && !reqSolidity && CanInit && user.Equipment.AttackStyle == Style.Melee)
            {
                var inst = Caller.Level.InstanceCheckPosition(Caller.X + Dir.X, Caller.Y + Dir.Y, Caller, reqSolidity);
                if (inst?.IsAttackable() ?? false)
                {
                    var style = user.Equipment.AttackStyle;
                    var attack = new Attack(
                        user.Server,
                        style,
                        user.Equipment.AttackSpeed,
                        user.Equipment.AttackRange,
                        player.GetAcc(style),
                        player.GetDmg(style),
                        user.Equipment.GetAttackProperties());

                    if (Caller.CanAffect(inst, attack))
                    {
                        // Check whether caller has waited long enough to attack with its current weapon
                        if (player.AttackTimer >= player.GetAttackSpeed(style))
                        {
                            Caller.Affect(inst, attack);
                            Caller.AttackTimer = -1;
                            return true;
                        }
                    }
                }
            }

            return true;
        }
    }
}
