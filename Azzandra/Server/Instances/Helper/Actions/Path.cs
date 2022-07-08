using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Path
    {
        public class Node
        {
            public int X;
            public int Y;
            public int F;
            public int G;
            public int H;
            public Node Parent;

            public Node(int x, int y) { X = x; Y = y; }

            public Vector ToVector() => new Vector(X, Y);
            public override string ToString() => ToVector().ToString();
        }


        public Entity Entity;
        private Node _target;
        public Vector Target { get => _target.ToVector(); set => _target = new Node(value.X, value.Y); }
        public List<Node> PathList { get; private set; }
        public int Length => PathList == null ? 0 : PathList.Count;
        public bool MustReach;
        public Region RegionConstraint;
        

        /// <summary>
        /// This class creates an A* path to specified target.
        /// </summary>
        /// <param name="entity">What entity this A* is for.</param>
        /// <param name="target"></param>Target destination to move towards.
        /// <param name="mustReach">Whether algorythm is allowed to proceed if target is unreachable.</param>
        public Path(Entity entity, Vector target, bool mustReach = false)
        {
            Entity = entity;
            Target = target;
            MustReach = mustReach;
            
            // If the enemy is inside its wander region, put up a constraint to remain here.
            if (entity is Enemy enemy && enemy.IsInRangeFromPoint(enemy.BasePosition, enemy.WanderRange))
            {
                RegionConstraint = enemy.GetRegionAroundBasePos(enemy.WanderRange);
            }
                

            CalculatePath();
        }

        /// <summary>
        /// Calculates a list of nodes from start to target destination.
        /// Returns the path length.
        /// </summary>
        /// <returns></returns>
        public int CalculatePath()
        {
            //check for target node
            if (_target == null) return 0;

            //declare start node
            Node start = new Node(Entity.X, Entity.Y);
            start.H = ComputeHScore(start.X, start.Y, _target.X, _target.Y);
            start.F = start.H;

            //initialize lists
            Node current = null;
            var openList = new List<Node>();
            var closedList = new List<Node>();
            int g = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == _target.X && l.Y == _target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, _target.X, _target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }

                if (closedList.Count >= 400)
                {
                    if (MustReach) return 0;
                    else
                    {
                        //change destination to closest known node
                        lowest = closedList.Min(n => n.H);
                        current = closedList.First(n => n.H == lowest);
                        break;
                    }
                }
            }

            //trace path back to first node
            PathList = new List<Node>();
            while (current.Parent != null)
            {
                PathList.Insert(0, current);
                current = current.Parent;
            }
            PathList.Insert(0, current);

            return PathList.Count;
        }

        private int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        private List<Node> GetWalkableAdjacentSquares(int x, int y)
        {
            var potentialDirections = new List<Dir>()
            { new Dir(0, 1), new Dir(0, -1), new Dir(1, 0), new Dir(1, -1), new Dir(1, 1), new Dir(-1, 0), new Dir(-1, -1), new Dir(-1, 1) };

            var nodes = new List<Node>();
            foreach (var dir in potentialDirections)
            {
                // If there is a region constraint and the new pos is not inside this region: don't go here.
                if (RegionConstraint != null && !RegionConstraint.IsInRegion(dir.ToVector() + new Vector(x, y), Entity.Size))
                    continue;

                if (Entity.CanMoveUnobstructed(x, y, dir.X, dir.Y, false))
                    nodes.Add(new Node(x + dir.X, y + dir.Y));
            }
            return nodes;
        }


        /// <summary>
        /// Removes and returns the next vector step on the path. Returns null if there is no step (e.g. at the end of the line).
        /// </summary>
        /// <returns></returns>
        public Vector? GetNextStep()
        {
            if (PathList != null)
            {
                // Reached end of path.
                if (PathList.Count <= 1 || Entity.X == PathList.Last().X && Entity.Y == PathList.Last().Y)
                    return null;

                // Entity is not on path.
                if (PathList[0].X != Entity.X || PathList[0].Y != Entity.Y)
                {
                    // Try to calculate a new path
                    if (CalculatePath() <= 1)
                        return null;
                }


                // Remove current node, get next node.
                PathList.RemoveAt(0);
                var next = PathList[0];

                // Calculate offset to current position.
                return new Vector(next.X - Entity.X, next.Y - Entity.Y);
            }

            return null;
        }

        /// <summary>
        /// Retrieves the next vector step on the path. Does NOT remove it from the path list! Returns null if there is no step (e.g. at the end of the line).
        /// </summary>
        /// <returns></returns>
        public Vector? PeekNextStep()
        {
            if (PathList != null)
            {
                // Reached end of path.
                if (PathList.Count <= 1 || Entity.X == PathList.Last().X && Entity.Y == PathList.Last().Y)
                    return null;

                // Entity is not on path.
                if (PathList[0].X != Entity.X || PathList[0].Y != Entity.Y)
                {
                    // Try to calculate a new path
                    if (CalculatePath() <= 1)
                        return null;
                }

                return PathList[0].ToVector() - Entity.Position;
            }

            return null;
        }
    }
}
