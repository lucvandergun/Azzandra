using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CaveWorm : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;
        public override int Initiative => 18;
        private int DigTimer = 5;
        private int GetDigDelay() => Util.Random.Next(4) + 4;

        public CaveWorm(int x, int y) : base(x, y) { }

        public override Symbol GetSymbol() => new Symbol('c', Color.AntiqueWhite);


        public override EntityAction DetermineAggressiveAction()
        {
            /* This method overrides the general behaviour with the following:
             * If a random timer == 0, dig a hole and show up on a random walkable tile next to the target.
             */
            
            // Just to make sure: check whether target actually exists
            var target = Target.Combatant;
            if (target == null)
            {
                Target = null;
                return null;
            }

            if (DigTimer >= 0)
                DigTimer--;

            // Dig a hole and show up elsewhere:
            if (DigTimer <= 0 && CanMove())
            {
                var surroundingTiles = Vector.Dirs4.SelectMany(d => target.GetTiles().Select(t => t + d)).Distinct().Where(n => !target.GetTiles().Contains(n)).ToList();
                surroundingTiles.Shuffle();
                foreach (var node in surroundingTiles)
                {
                    if (CanExist(node.X, node.Y))
                    {
                        DigTimer = GetDigDelay();
                        // If the chosen position is not the same as the current position, "teleport" right to it.
                        if (Position != node)
                        {
                            if (target is Player player)
                                player.User.ShowMessage("<tan>" + ToStringAdress().CapFirst() + " tunnels through the ground and shows up right next to you.");
                            Position = node;
                        }
                        return null;
                    }
                }
            }

            return base.DetermineAggressiveAction();
        }


        public override void Load(byte[] bytes, ref int pos)
        {
            // Active
            DigTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            bytes.Insert(0, BitConverter.GetBytes(DigTimer));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
