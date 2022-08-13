using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DijkstraMap : BehaviourMap
    {
        public IEnumerable<Instance> Targets { get; private set; }

        public float[,] FloatMatrix { get; protected set; }


        public DijkstraMap(Level level, Entity caller, IEnumerable<Instance> targets) : base(level, caller)
        {
            Targets = targets;
        }

        public void CombineWith(BehaviourMap other)
        {
            for (int i, j = 0; j < Level.MapHeight; j++)
            {
                for (i = 0; i < Level.MapWidth; i++)
                {
                    if (Matrix[i, j] < int.MaxValue && other.Matrix[i, j] < int.MaxValue)
                        Matrix[i, j] += other.Matrix[i, j];
                }
            }
        }

        public void MultiplyWith(float factor)
        {
            var map = new float[Level.MapWidth, Level.MapHeight];
            for (int i, j = 0; j < Level.MapHeight; j++)
            {
                for (i = 0; i < Level.MapWidth; i++)
                {
                    FloatMatrix[i, j] = FloatMatrix[i, j] * factor;
                }
            }
        }

        public void ToIntMatrix()
        {
            Matrix = new int[Level.MapWidth, Level.MapHeight];
            for (int i, j = 0; j < Level.MapHeight; j++)
            {
                for (i = 0; i < Level.MapWidth; i++)
                {
                    Matrix[i, j] = (int)FloatMatrix[i, j];
                }
            }
        }




        class Node
        {
            public Vector Pos;
            public int Dist;
            
            public Node(Vector v, int dist)
            {
                Pos = v;
                Dist = dist;
            }
        }

        /// <summary>
        /// Create a mountain of raw distances from the goal tiles.
        /// </summary>
        public void CreateMap()
        {
            // First fill the matrix with -1, then update all fields
            Matrix = new int[Level.MapWidth, Level.MapHeight];
            Matrix.Populate(int.MaxValue);
            var goalTiles = Targets.SelectMany(t => t.GetTiles());

            // Add all goal tiles to PQ, set their fields in matrix to 0
            var PQ = new Queue<Node>();
            goalTiles.ToList().ForEach(t => {
                PQ.Enqueue(new Node(t, 0));
                //Matrix[t.X, t.Y] = 0;
                });

            while (PQ.Count > 0)
            {
                // Temove first node from PQ, set matrix value
                var current = PQ.Dequeue();
                Matrix[current.Pos.X, current.Pos.Y] = current.Dist;

                // Add all reachable tiles to PQ
                foreach (var dir in Vector.Dirs8)
                {
                    var pos = current.Pos + dir;
                    if (!Level.IsInMapBounds(pos.X, pos.Y))
                        continue;

                    // Add the new node to the PQ if it is reachable by the caller, and not yet (to be) investigated.
                    if (Matrix[pos.X, pos.Y] == int.MaxValue && !PQ.Any(n => n.Pos == pos))
                    {
                        if (Caller.CanMoveUnobstructed(current.Pos.X, current.Pos.Y, dir.X, dir.Y, false))
                        {
                            PQ.Enqueue(new Node(pos, current.Dist + 1));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries for all eight surrounding direction what the best tile to move on is.
        /// </summary>
        /// <returns>The best possible direction.</returns>
        public Vector GetStep()
        {
            // 'Roll' uphill, get the first step
            if (!Level.IsInMapBounds(Caller.X, Caller.Y))
                return Vector.Zero;

            var currentValue = Matrix[Caller.X, Caller.Y];
            var possibleDirs = Vector.Dirs9.Where(d =>
                Level.IsInMapBounds(Caller.X + d.X, Caller.Y + d.Y));
            var lowestValue = possibleDirs.Min(d => Matrix[Caller.X + d.X, Caller.Y + d.Y]);
            var bestDir = possibleDirs.FirstOrDefault(d => Matrix[Caller.X + d.X, Caller.Y + d.Y] == lowestValue);

            return bestDir;

            //foreach (var dir in Vector.Dirs8)
            //{
            //    var pos = Caller.Position + dir;
            //    if (!Level.IsInMapBounds(pos.X, pos.Y)) continue;
            //    if (currentValue > Matrix[pos.X, pos.Y])
            //        return dir;
            //}
            //return Vector.Zero;
        }



        /// <summary>
        /// Creates simple Matrix with all maxvalues and 0 for the target/goal tiles
        /// </summary>
        public void CreateMap2()
        {
            // First fill the matrix with -1, then update all fields
            FloatMatrix = new float[Level.MapWidth, Level.MapHeight];
            FloatMatrix.Populate(int.MaxValue);

            // Add all goal tiles to PQ, set their fields in matrix to 0
            var goalTiles = Targets.SelectMany(t => t.GetTiles());
            var PQ = new Queue<Node>();
            goalTiles.ToList().ForEach(t => {
                PQ.Enqueue(new Node(t, 0));
                FloatMatrix[t.X, t.Y] = 0;
            });
        }



        /// <summary>
        /// Iterates over the current Matrix by updating the states, until convergence.
        /// </summary>
        public void IterateOverMap()
        {
            var copy = FloatMatrix.CreateCopy();
            while (true)
            {
                bool edited = false;
                for (int i, j = 0; j < Level.MapHeight; j++)
                {
                    for (i = 0; i < Level.MapWidth; i++)
                    {
                        var possibleDirs = Vector.Dirs8.Where(d =>
                            Level.IsInMapBounds(i + d.X, j + d.Y) &&
                            Caller.CanMoveUnobstructed(i, j, d.X, d.Y, true));

                        if (possibleDirs.Count() <= 0)
                            continue;

                        var lowestValue = possibleDirs.Min(d => FloatMatrix[i + d.X, j + d.Y]);
                        //var lowestNeighbour = possibleDirs.FirstOrDefault(d => Matrix[i + d.X, j + d.Y] == lowestValue);
                        

                        // Update the current field if its much higher than any adjacent
                        if (lowestValue < FloatMatrix[i, j] - 1)
                        {
                            copy[i, j] = lowestValue + 1;
                            if (!edited) edited = true;
                        }
                    }
                }

                if (edited)
                    FloatMatrix = copy;
                else
                    break;
            }
        }
    }
}
