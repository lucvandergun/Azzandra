using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Path2
    {
        public class Node
        {
            public Vector Position;
            public int X => Position.X;
            public int Y => Position.Y;

            public float F;
            public int G;
            public float H;
            public Node Parent;

            public Node(int x, int y) { Position = new Vector(x, y); }
            public Node(Vector v) { Position = v; }

            public Vector ToVector() => new Vector(X, Y);
            public override string ToString() => Position.ToString();
        }


        public Entity Entity;
        public Instance TargetInstance;
        public IEnumerable<Vector> Target;
        public List<Node> PathList { get; private set; }
        public int Length => PathList == null ? 0 : PathList.Count;
        public bool MustReach;
        public Region RegionConstraint;

        private Vector GetClosestTargetNode(Node node)
        {
            var minDist = Target.Min(t => ComputeTargetCloseness(t, node.Position)); //ComputeHScore(t, node.Position)
            var pos = Target.FirstOrDefault(t => ComputeTargetCloseness(t, node.Position) == minDist);
            return pos;
        }

        private float ComputeTargetCloseness(Vector a, Vector t)
        {
            return (t - a).ChebyshevLength() + ((t - a).IsOrthogonal() ? 0.5f : 0f);
        }


        /// <summary>
        /// This class creates an A* path to specified target.
        /// </summary>
        /// <param name="entity">What entity this A* is for.</param>
        /// <param name="target"></param>Target destination to move towards.
        /// <param name="mustReach">Whether algorythm is allowed to proceed if target is unreachable.</param>
        /// <param name="regionConstraint">Whether to constraint the movable region to the NPC's WanderRange. Only works for NPC's.</param>
        public Path2(Entity entity, Instance target, bool mustReach = false, bool regionConstraint = false)
        {
            Entity = entity;
            TargetInstance = target;
            Target = target.GetTiles();
            MustReach = mustReach;
            
            // If the enemy is inside its wander region, put up a constraint to remain here.
            if (regionConstraint && entity is Enemy enemy && enemy.IsInRangeFromPoint(enemy.BasePosition, enemy.WanderRange))
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
            //Entity.Level.Server.User.ShowMessage(Entity.ID + ": recalculated");
            
            // check for target node
            if (Target == null) return 0;

            // declare start node
            Node start = new Node(Entity.X, Entity.Y);
            var target = GetClosestTargetNode(start);
            start.H = ComputeHScore(start.Position, target);
            start.F = start.H;

            // initialize lists
            Node current = null;
            var openList = new List<Node>();
            var closedList = new List<Node>();
            int g = 0;
            int tilesChecked = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);
                tilesChecked++;

                // remove it from the open list
                openList.Remove(current);

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.Any(l => l.Position == adjacentSquare.Position))
                        continue;

                    // if it's not already in the open list...
                    if (!openList.Any(l => l.Position == adjacentSquare.Position))
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        target = GetClosestTargetNode(start);
                        adjacentSquare.H = ComputeHScore(adjacentSquare.Position, target);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // if we added the destination to the closed list, we've found a path
                        if (closedList.Any(l => Target.Contains(adjacentSquare.Position)))
                        {
                            break;
                        }

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

                // Quit the algorithm after x iterations: the target could not be found - not accessible or way too far away.
                if (tilesChecked >= 200)
                    break;
            }


            // Find the node closest to the target, or better yet, the target itself:
            var closest = closedList.Min(n => n.H);
            current = closedList.First(n => n.H == closest);

            // Return no path if it didn't catch the target, even though it should have:
            if (MustReach && !Target.Contains(current.Position))
                return 0;

            //if (!Target.Contains(current.Position))
            //    Entity.Level.Server.User.ThrowError(Entity.ToString().CapFirst() + " " + Entity.ID + ", Found best: " + current.Position + ", list: " + closedList.Stringify());

            // Trace path back to first node
            PathList = new List<Node>();
            while (current.Parent != null)
            {
                PathList.Insert(0, current);
                current = current.Parent;
            }
            PathList.Insert(0, current);

            // DEBUG - set sight squares:
            //if (!PathList.Any(n => Target.Contains(n.Position)))
            //    Entity.SightSquares = PathList.Select(n => n.Position);

            return PathList.Count;
        }

        private float ComputeHScore(Vector pos, Vector target)
        {
            return (target - pos).OrthogonalLength();// + ((target - pos).Absolute() == Vector.One ? 0.5f : 0f);
        }

        private List<Node> GetWalkableAdjacentSquares(int x, int y)
        {
            var potentialDirections = Vector.Dirs8;

            var nodes = new List<Node>();
            foreach (var dir in potentialDirections)
            {
                var newPos = dir + new Vector(x, y);

                // Allow free-passes for target tiles themselves: (they don't have to be walkable)
                if (!Target.Contains(newPos))
                {
                    // If there is a region constraint and the new pos is not inside this region: don't go here.
                    if (RegionConstraint != null && !RegionConstraint.IsInRegion(newPos, Entity.Size))
                        continue;

                    if (!Entity.CanMoveUnobstructed(x, y, dir.X, dir.Y, true, !Entity.CanOpenDoors()))
                        continue;
                }
                                    
                nodes.Add(new Node(newPos));
            }
            return nodes;
        }


        /// <summary>
        /// Removes and returns the next vector step on the path. Returns null if there is no step (e.g. at the end of the line).
        /// </summary>
        /// <returns></returns>
        public Vector? GetNextStep()
        {
            var targetTiles = TargetInstance.GetTiles();
            if (Target.Intersect(targetTiles).Count() < targetTiles.Count())
            {
                Target = targetTiles;
                CalculatePath();
            }
            
            if (PathList != null)
            {
                // Reached end of path.
                if (PathList.Count <= 1 || Entity.Position == PathList.Last().Position)
                    return null;

                // Entity is not on path.
                if (PathList[0].Position != Entity.Position || PathList[0].Y != Entity.Y)
                {
                    // Try to calculate a new path
                    if (CalculatePath() <= 1)
                        return null;
                }


                // Remove current node, get next node.
                PathList.RemoveAt(0);
                var next = PathList[0];

                // Calculate offset to current position.
                return next.Position - Entity.Position;
            }

            return null;
        }
    }
}
