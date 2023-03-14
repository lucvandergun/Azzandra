using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class NPC : Entity
    {
        public Vector? BasePosition { get; set; }   // Base position to stick location to:  null = no restricitons
        public bool IsHaunting = false;
        public virtual int WanderRange => 4;        // The distance from the base position the npc is allowed to target a wander path to
        
        public List<NPC> Group { get; set; }
        
        public NPC(int x, int y) : base(x, y)
        {

        }

        public override void Load(byte[] bytes, ref int pos)
        {
            int x, y;
            x = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            y = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            if (x != 0 || y != 0)   // base pos wasn't 0
                BasePosition = new Vector(x, y);

            IsHaunting = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[9];

            var basePos = BasePosition ?? Vector.Zero;
            bytes.Insert(0, BitConverter.GetBytes(basePos.X));
            bytes.Insert(4, BitConverter.GetBytes(basePos.Y));
            bytes.Insert(8, BitConverter.GetBytes(IsHaunting));

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();
            Group?.Remove(this);
        }

        public virtual void EvaluateBasePositionByGroup()
        {
            // Set base pos as average group position
            if (Group != null)
            {
                var p = Vector.Zero;
                foreach (var inst in Group)
                    p += inst.Position;
                BasePosition = p / Group.Count;
                return;
            }
        }

        /// <summary>
        /// Calculates the wander area, returns null if no restrictions, tuple<topleft, size> otherwise
        /// </summary>
        /// <returns>The wander area by top-left and size of area</returns>
        public Region GetRegionAroundBasePos(int range)
        {
            if (BasePosition == null)
                return null;
            else
            {
                return new Region(BasePosition.Value - new Vector(range), new Vector(range * 2 + 2) - Size);    // +2 to counter for middle square of range & size
            }
        }

        protected virtual Vector PickWanderTarget()
        {
            if (BasePosition == null)
            {
                var target = new Vector(
                    Util.Random.Next(X - WanderRange, X + WanderRange + 2 - GetW()),
                    Util.Random.Next(Y - WanderRange, Y + WanderRange + 2 - GetH()));
                return target;
            }
            else
            {
                var b = BasePosition.Value;
                var target = new Vector(
                    Util.Random.Next(b.X - WanderRange, b.X + WanderRange + 2 - GetW()),
                    Util.Random.Next(b.Y - WanderRange, b.Y + WanderRange + 2 - GetH()));
                return target;
            }
        }

        public virtual EntityAction DetermineRegularAction()
        {
            // Perform any standing actions:
            if (Action is ActionPath) //!= null
                return Action;

            // Pick a location target and set action to move towards it:
            EvaluateBasePositionByGroup();
            Vector target = PickWanderTarget();

            if (CanTargetTile(Level.GetTile(target)) && CanExist(target.X, target.Y))
            {
                return new ActionPath(this, target, true, true);
            }

            return null;
        }


        protected virtual bool CanTargetTile(Tile tile)
        {
            return CanStandOnTile(tile, GetMovementType());
        }

        /// <summary>
        /// This method checks whether the target tile is in wander range to the base position.
        /// No base position automatically returns true.
        /// </summary>
        /// <param name="target">The target tile vector</param>
        /// <returns>Whether tile is close enough to base</returns>
        protected virtual bool IsTargetTileInRange(Vector target)
        {
            if (BasePosition == null)
                return true;

            var dist = (BasePosition.Value - target).Sign();
            return (dist.X <= WanderRange && dist.Y <= WanderRange);
        }



        // === Rendering: Adds Debug info for NPC's === \\

        public override void DrawView(SpriteBatch sb, Vector2 viewOffset, Server server, float lightness)
        {
            base.DrawView(sb, viewOffset, server, lightness);

            // Draw Path Target
            if (server.GameClient.IsDevMode && server.GameClient.IsDebug && server.User.Target == this)
            {
                if (Action is ActionPath apath)
                {
                    DrawRectangle(viewOffset, server, apath.Path.Target, Size);
                }

                var centerPos = (BasePosition != null ? BasePosition.Value : Position);
                DrawRectangle(viewOffset, server, centerPos - new Vector(WanderRange), new Vector(2 * WanderRange + 1));
            }
        }

        public void DrawRectangle(Vector2 viewOffset, Server server, Vector pos, Vector size, Color? color = null)
        {
            Vector2 s = size.ToFloat() * GameClient.GRID_SIZE;
            var drawPos = pos.ToFloat() * GameClient.GRID_SIZE + viewOffset;
            var rect = Display.MakeRectangle(drawPos, s);

            Display.DrawOutline(rect, color == null ? GetSymbol().Color : color.Value);
        }
    }
}
