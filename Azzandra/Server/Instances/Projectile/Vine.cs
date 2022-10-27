using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Vine : TargetProjectile
    {
        public Vector[] Nodes;
        public int TargetInstanceID { get; protected set; } = -1;

        public override IEnumerable<Vector> GetTiles() => Nodes.AsEnumerable();

        public override Symbol GetSymbol() => new Symbol('&', Color.Green);
        public override bool IsInteractable() { return true; }


        // == Targeting == \\
        protected void RemoveTarget() => TargetInstanceID = -1;
        protected bool TargetExists() => TargetInstanceID != -1 && Level.GetInstanceByID(TargetInstanceID) != null;
        public bool HasTarget() => TargetInstanceID >= 0;
        protected Entity GetTarget()
        {
            if (TargetInstanceID > 5000) Level.Server.ThrowError(ID + " has incorrect target id: " + TargetInstanceID + "");

            // Get target instance from Level
            var t = Level.GetInstanceByID(TargetInstanceID);

            if (t is Entity c)
                return c;

            return null;
        }



        public Vine(Instance origin, Instance target) : base(origin, target)
        {
            // Set all the nodes
            Nodes = Util.CastRay(origin.GetTiles(), target.GetTiles(), false, true).Concat(target.GetTiles()).Distinct().ToArray();
            // Set target instance id:
            TargetInstanceID = target.ID;
        }

        public Vine(int x, int y) : base(x, y)
        {

        }

        public override void Turn()
        {
            base.Turn();

            // Check target and origin still exists..
            if (HasTarget() && !TargetExists())
            {
                Destroy();
            }
                

            // Entangle target: destroy self if no target or target is no longer touching.
            var target = GetTarget();
            if (target == null || target.GetTiles().Intersect(Nodes).Count() <= 0)
            {
                Destroy();
            }
            else
            {
                var effect = new StatusEffects.Frozen(1, 2, "entangled"); // duration is 2, otherwise it gets immediately removed once the player turn commences!
                target.AddStatusEffect(effect, true);
            }
        }


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Target ID
            TargetInstanceID = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Nodes
            int amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Nodes = new Vector[amt];
            for (int i = 0; i < amt; i++)
                Nodes[i] = Vector.Load(bytes, ref pos);

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[8];

            // Target ID
            bytes.Insert(0, BitConverter.GetBytes(TargetInstanceID));

            // Nodes
            int amt = Nodes.Length;
            bytes.Insert(4, BitConverter.GetBytes(amt));
            foreach (var node in Nodes)
                bytes = bytes.Concat(node.ToBytes()).ToArray();

            return bytes.Concat(base.ToBytes()).ToArray();
        }



        public override void Interact(Entity entity)
        {
            // Allow any entity to cut the vine.
            if (Util.Random.Next(3) == 0)
            {
                var actor = entity is Player ? "You cut" : entity.ToStringAdress().CapFirst() + " cuts";
                Level.Server.User.ShowMessage("<medblue>" + actor + " the vine!");
                Destroy();
            }
            else
            {
                var actor = entity is Player ? "You fail" : entity.ToStringAdress().CapFirst() + " fails";
                Level.Server.User.ShowMessage("<rose>" + actor + " to cut the vine!");
            }
        }


        public override void Draw(SpriteBatch sb, Vector2 pos, float lightness)
        {
            var sprite = GetSprite();
            if (sprite == null) return;

            // Draw all nodes at once at their relative positions.
            if (Nodes != null)
            {
                foreach (var node in Nodes)
                {
                    var symbol = GetSymbol();
                    var relativeDiff = (node - Position).ToFloat() * ViewHandler.GRID_SIZE;
                    Display.DrawSprite(pos + relativeDiff, sprite, Color.White, 1f, 0f);
                    //Display.DrawStringCentered(pos + relativeDiff, symbol.Char, Assets.Gridfont, symbol.Color);
                }
            }
        }
    }
}
