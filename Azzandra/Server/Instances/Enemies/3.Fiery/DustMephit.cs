using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DustMephit : Enemy
    {
        public override EntityType EntityType => EntityType.Rock;
        public override int GetMoveDelay() => 4;

        private bool FireStraight = true;

        public DustMephit(int x, int y) : base(x, y) { }

        public override EntityAction DetermineAggressiveAction()
        {
            // Just to make sure: check whether target actually exists
            var target = Target.Combatant;
            if (target == null)
            {
                Target = null;
                return null;
            }

            
            // Spawn dust clouds
            var dist = DistanceTo(target);

            // Move from under target
            if (dist.X == 0 && dist.Y == 0)
                return new ActionMove(this, Dir.Random.ToVector());

            // Fire if in line with target:
            //if (!FireStraight && dist.X == dist.Y || FireStraight && (dist.X == 0 || dist.Y == 0))
            {
                if (AttackTimer >= 4)
                {
                    var angles = (dist.X == 0 || dist.Y == 0) ? Vector.Dirs4 : dist.X == dist.Y ? Vector.Dirs4Diagonal : Util.Random.Next(2) == 1 ? Vector.Dirs4 : Vector.Dirs4Diagonal;
                    foreach (var angle in angles)
                    {
                        Level.CreateInstance(new DustCloud(X, Y, angle));
                    }
                    FireStraight = !FireStraight;

                    AttackTimer = 0;
                    return null;
                }

                //return null;// new ActionMove(target, Vector.Zero, true);
            }

            return new ActionMoveTo(this, target);

            //// If not valid position - path to be in line with the target (according to FireStraight)
            //var allTiles = new List<Vector>();
            //int range = 5;
            //for (int i, j = target.Position.Y - range; j < target.Position.Y + target.GetH() + range; j++)
            //    for (i = target.Position.X - range; i < target.Position.X + target.GetW() + range; i++)
            //        allTiles.Add(new Vector(i, j));

            //var feasibleTiles = allTiles.Where(t => FireStraight ? target.GetTiles().Any(tt => (t - tt).IsOrthogonal()) : target.GetTiles().Any(tt => (t - tt).IsPerfectDiagonal()));
            //feasibleTiles = feasibleTiles.Where(t => CanExist(t.X, t.Y));
            //feasibleTiles.ToList().Sort((a, b) => (a.X - X) - (b.X - X));
            //var best = feasibleTiles.FirstOrDefault();
            //if (best != Vector.Zero)
            //    return new ActionPath(this, best, true);

            //return null;
        }

        public override Symbol GetSymbol() => new Symbol('d', Color.Tan);

        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            FireStraight = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(FireStraight);

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
