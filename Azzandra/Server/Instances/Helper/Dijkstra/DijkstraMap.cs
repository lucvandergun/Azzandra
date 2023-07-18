using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    /// <summary>
    /// DijkstraMap that contains a 2D float array (matrix)
    /// Contains a Matrix (2D int array)
    /// </summary>
    public class DijkstraMap
    {
        public Entity Caller { get; protected set; }
        public Level Level { get; protected set; }
        //public int Width { get; private set; }
        //public int Height { get; private set; }
        public float[,] Matrix { get; protected set; }
        public Vector Size, Offset;
        public const int RANGE = 6;
        public readonly Vector Range = new Vector(RANGE);

        public bool IsOnMatrix(Vector pos) => pos >= Offset && pos < Offset + Size;
        public bool IsOnMatrix(Instance inst) => inst.Position >= Offset && inst.Position + inst.Size < Offset + Size;

        public IEnumerable<Instance> Targets { get; private set; }

        public float GetMinValue()
        {
            float min = 1;
            for (int i, j = 0; j < Size.Y; j++)
                for (i = 0; i < Size.X; i++)
                    if (Matrix[i, j] < min) min = Matrix[i, j];
            return min;
        }


        public DijkstraMap(Entity caller, IEnumerable<Instance> targets)
        {
            Level = caller.Level;
            Caller = caller;
            Targets = targets;

            // Calculate matrix size and offset;
            Offset = Vector.Max(Vector.Zero, caller.Position - Range);
            Size = Vector.Min(new Vector(Level.MapWidth - 1, Level.MapHeight - 1), caller.Position + Range) + Caller.Size - Offset;
        }

        /// <summary>
        /// Get the value in the matrix if it exists.
        /// </summary>
        /// <param name="pos">The absolute coordinates, not relative to the offset of the matrix!</param>
        /// <returns></returns>
        public float GetValue(Vector pos)
        {
            if (pos >= Offset && pos < Offset + Size)
                return Matrix[pos.X - Offset.X, pos.Y - Offset.Y];

            return 1; // right value here?
        }



        // ========== Matrix Handlers: ========== \\

        //public void CombineWith(DijkstraMap other)
        //{
        //    for (int i, j = 0; j < Level.MapHeight; j++)
        //    {
        //        for (i = 0; i < Level.MapWidth; i++)
        //        {
        //            if (Matrix[i, j] < int.MaxValue && other.Matrix[i, j] < int.MaxValue)
        //                Matrix[i, j] += other.Matrix[i, j];
        //        }
        //    }
        //}

        public void MultiplyWith(float factor)
        {
            var map = new float[Size.X, Size.Y];
            for (int i, j = 0; j < Size.Y; j++)
            {
                for (i = 0; i < Size.X; i++)
                {
                    Matrix[i, j] *= factor;
                }
            }
        }

        //public void CalculateIntMatrix()
        //{
        //    Matrix = new int[Level.MapWidth, Level.MapHeight];
        //    for (int i, j = 0; j < Level.MapHeight; j++)
        //    {
        //        for (i = 0; i < Level.MapWidth; i++)
        //        {
        //            Matrix[i, j] = (int)FloatMatrix[i, j];
        //        }
        //    }
        //}





        // ========== Matrix Creation Methods: ========== \\

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
        /// This is basically a flood-fill algorithm from each of the goal tiles (in 'Target'),
        /// but it also tracks the distance of each tile to the goal.
        /// </summary>
        public void CreateMap()
        {
            int defaultValue = -1;
            
            // First fill the matrix with MAX_VALUE, then update all fields
            Matrix = new float[Size.X, Size.Y];
            Matrix.Populate(defaultValue);
            var goalTiles = Targets.SelectMany(t => t.GetTiles()).Where(t => IsOnMatrix(t));

            // The priority queue starts with the goal tiles, and then subsequently adds neigbouring tiles to the priority queue.
            var PQ = new Queue<Node>();
            goalTiles.ToList().ForEach(t => PQ.Enqueue(new Node(t, 0)));

            while (PQ.Count > 0)
            {
                // Remove first/top node from the queue, set its matrix value to the distance it has to the goals
                var current = PQ.Dequeue();
                Matrix[current.Pos.X - Offset.X, current.Pos.Y - Offset.Y] = current.Dist;

                // Add all reachable neigbour tiles to PQ
                foreach (var step in Vector.Dirs8)
                {
                    var pos = current.Pos + step;
                    if (!IsOnMatrix(pos))
                        continue;

                    // Add the new node to the PQ if it is reachable by the caller, and not yet (to be) investigated.
                    if (Matrix[pos.X - Offset.X, pos.Y - Offset.Y] == defaultValue && !PQ.Any(n => n.Pos == pos))
                    {
                        if (Caller.CanMoveUnobstructed(current.Pos.X, current.Pos.Y, step.X, step.Y, true, !Caller.CanOpenDoors()) || Caller.Position == pos)
                        {
                            PQ.Enqueue(new Node(pos, current.Dist + 1)); // Increase distance by 1.
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries for all eight surrounding direction what the best direction is to move in.
        /// </summary>
        /// <returns>The best direction to move in.</returns>
        public Vector GetStep()
        {
            // Roll downhill to get the next step
            if (Caller.Position < Offset || Caller.Position + Caller.Size >= Offset + Size)
                return Vector.Zero;

            var currentValue = Matrix[Caller.X - Offset.X, Caller.Y - Offset.Y];
            
            var possibleDirs = Vector.Dirs9.Where(d =>
                    Caller.CanMoveUnobstructed(Caller.X, Caller.Y, d.X, d.Y, true, !Caller.CanOpenDoors()) &&
                    Caller.Position + d >= Offset &&
                    Caller.Position + Caller.Size + d < Offset + Size);

            var lowestValue = possibleDirs.Min(d => Matrix[Caller.X + d.X - Offset.X, Caller.Y + d.Y - Offset.Y]);
            var bestDir = possibleDirs.FirstOrDefault(d => Matrix[Caller.X + d.X - Offset.X, Caller.Y + d.Y - Offset.Y] == lowestValue);

            return bestDir;
        }



        ///// <summary>
        ///// Creates simple Matrix with all maxvalues and 0 for the target/goal tiles
        ///// </summary>
        //public void CreateMap2()
        //{
        //    // First fill the matrix with -1, then update all fields
        //    FloatMatrix = new float[Level.MapWidth, Level.MapHeight];
        //    FloatMatrix.Populate(int.MaxValue);

        //    // Add all goal tiles to PQ, set their fields in matrix to 0
        //    var goalTiles = Targets.SelectMany(t => t.GetTiles());
        //    var PQ = new Queue<Node>();
        //    goalTiles.ToList().ForEach(t => {
        //        PQ.Enqueue(new Node(t, 0));
        //        FloatMatrix[t.X, t.Y] = 0;
        //    });
        //}



        /// <summary>
        /// Iterates over the current Matrix by updating the states, until convergence.
        /// </summary>
        public void IterateOverMapOld()
        {
            var copy = Matrix.CreateCopy();
            while (true)
            {
                bool edited = false;
                for (int i, j = 0; j < Size.Y; j++)
                {
                    for (i = 0; i < Size.X; i++)
                    {
                        if (copy[i, j] > 0) continue;

                        var possibleDirs = Vector.Dirs9.Where(d =>
                            IsOnMatrix(new Vector(i + d.X, j + d.Y) + Offset) &&
                            Caller.CanMoveUnobstructed(i + Offset.X, j + Offset.Y, d.X, d.Y, true, !Caller.CanOpenDoors()));

                        if (possibleDirs.Count() <= 0)
                            continue;

                        // Update the current field if its much higher than any adjacent
                        var lowestValue = possibleDirs.Min(d => Matrix[i + d.X, j + d.Y]);
                        if (lowestValue < Matrix[i, j] - 1) // If the lowest is smaller than current value - 1:
                        {
                            copy[i, j] = lowestValue + 1;
                            if (!edited)
                                edited = true;
                        }
                    }
                }

                if (edited)
                    Matrix = copy;
                else
                    break;
            }
        }

        /// <summary>
        /// Iterates over the current Matrix by updating the states, until convergence.
        /// </summary>
        public void IterateOverMap()
        {
            /// Assumpption:
            /// - two orthogonally adjacent tiles are (of value <= 0) always reachable
            /// - two diagonal tiles are reachable if both shared neigbours are, otherwise check using 'CanMoveUnobstructed'
            
            var copy = Matrix.CreateCopy();
            bool edited; Vector pos;
            while (true)
            {
                edited = false;
                for (int i, j = 0; j < Size.Y; j++)
                {
                    for (i = 0; i < Size.X; i++)
                    {
                        if (copy[i, j] > 0f) continue;
                        pos = new Vector(i, j);

                        var possibleDirs = Vector.Dirs8.Where(d => GetValue(pos + d + Offset) <= 0f);
                        possibleDirs = possibleDirs.Where(d =>
                            d.IsOrthogonal() ||
                            Caller.CanMoveUnobstructed(i + Offset.X, j + Offset.Y, d.X, d.Y, true, !Caller.CanOpenDoors()));

                        if (possibleDirs.Count() <= 0)
                            continue;

                        // Update the current field if its much higher than any adjacent
                        var lowestValue = possibleDirs.Min(d => Matrix[i + d.X, j + d.Y]);
                        if (lowestValue < Matrix[i, j] - 1) // If the lowest is smaller than current value - 1:
                        {
                            copy[i, j] = lowestValue + 1;
                            if (!edited)
                                edited = true;
                        }
                    }
                }

                if (edited)
                    Matrix = copy;
                else
                    break;
            }
        }
    }
}
