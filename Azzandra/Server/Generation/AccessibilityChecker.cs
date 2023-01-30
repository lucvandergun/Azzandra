using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public static class AccessibilityChecker
    {
        protected class Node
        {
            public int X;
            public int Y;
            public int F;
            public int G;
            public int H;
            public Node Parent;

            public Node(int x, int y) { X = x; Y = y; }
            public Node(Vector v) { X = v.X; Y = v.Y; }
            public Vector ToVector() => new Vector(X, Y);

            public int OrthogonalDistanceTo(Node other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
            }
            public bool IsNextToTarget(List<Vector> targetTiles)
            {
                foreach (var tile in targetTiles)
                {
                    if ((tile - ToVector()).OrthogonalLength() <= 1)
                        return true;
                }
                return false;
            }
            public int ChebyshevDistanceTo(Node other)
            {
                return Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
            }
        }


        /// <summary>
        /// Calculates a list of nodes from start to target destination.
        /// Returns whether the target is reached.
        /// </summary>
        public static bool IsAccessible(Level level, Area area, List<Vector> startTiles, List<Vector> targetTiles, List<Vector> obstructingTiles, bool draw = false)
        {
            if (startTiles == null || targetTiles == null)
                return false;
            
            // Initialize nodes:
            var startTile = startTiles[0];
            var closestTargetDistance = targetTiles.Min(n => (n - startTile).ChebyshevLength());
            Node start = new Node(startTile);
            Node target = new Node(targetTiles.First(n => (n - startTile).ChebyshevLength() == closestTargetDistance));

            var areaTiles = area.Nodes;
            if (obstructingTiles == null) obstructingTiles = new List<Vector>();


            // Initialize lists:
            Node current = null;
            var openList = new List<Node>();
            var closedList = new List<Node>();
            int g = 0;

            start.H = ComputeHScore(start, target);
            start.F = start.H;
            openList.Add(start);

            while (openList.Count > 0)
            {
                // Get node with the lowest F score - remove from open & place in closed
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);
                closedList.Add(current);
                openList.Remove(current);

                // Completed if next to or at any of target tiles:
                if (closedList.FirstOrDefault(l => l.IsNextToTarget(targetTiles)) != null)
                {
                    if (draw) DrawPath(area, current);
                    return true;
                }

                var adjacentNodes = GetWalkableAdjacentSquares(level, current, targetTiles, areaTiles, obstructingTiles);
                g++;

                foreach (var adjacent in adjacentNodes)
                {
                    // If this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(n => n.X == adjacent.X
                            && n.Y == adjacent.Y) != null)
                        continue;

                    // If it's not in the open list...
                    if (openList.FirstOrDefault(n => n.X == adjacent.X && n.Y == adjacent.Y) == null)
                    {
                        // Compute its score, set the parent
                        adjacent.G = g;
                        adjacent.H = ComputeHScore(adjacent, target);
                        adjacent.F = adjacent.G + adjacent.H;
                        adjacent.Parent = current;

                        // And add it to the open list
                        openList.Insert(0, adjacent);
                    }
                    else
                    {
                        // Test if using the current G score makes the adjacent square's F score
                        // Lower, if yes update the parent because it means it's a better path
                        if (g + adjacent.H < adjacent.F)
                        {
                            adjacent.G = g;
                            adjacent.F = adjacent.G + adjacent.H;
                            adjacent.Parent = current;
                        }
                    }
                }

                if (closedList.Count >= 400)
                {
                    return false;
                }
            }

            // At this point the A* has failed to find a valid path

            if (draw && level.Server.GameClient.IsDevMode)
            {
                level.Server.ThrowError("Accessibility check failed: " + start.ToVector() + " to " + target.ToVector() + ", Area: " + area.ID);
                DrawPath(area, current);
            }
            return false;
        }


        private static int ComputeHScore(Node node, Node target)
        {
            return node.OrthogonalDistanceTo(target);
        }


        private static List<Node> GetWalkableAdjacentSquares(Level world, Node node, List<Vector> targetTiles, List<Vector> areaNodes, List<Vector> obstructingTiles)
        {
            // Get potential directions to move in:
            var potentialDirs = new List<Dir>() { new Dir(0, 1), new Dir(0, -1), new Dir(1, 0), new Dir(-1, 0) };
            var nodes = new List<Node>();

            // Try each:
            foreach (var dir in potentialDirs)
            {
                var newNode = new Node(node.X + dir.X, node.Y + dir.Y);

                // If area's nodes contains node or node equals target, and node is not one of the obstructing tiles
                if ( (areaNodes.Contains(newNode.ToVector()) || targetTiles.Contains(newNode.ToVector())) && !obstructingTiles.Contains(newNode.ToVector()))
                {
                    if (IsWalkable(world, newNode.ToVector()))
                        nodes.Add(newNode);
                }
            }
            return nodes;
        }

        public static bool IsWalkable(Level world, Vector node)
        {
            // Check tileID is walkable
            if (!IsWalkableTile(world, node))
                return false;

            // Check tile is not occupied by solid instance
            foreach (var inst in world.ActiveInstances)
            {
                if (!CanWalkOverInstance(inst))
                    if (inst.GetTiles().Contains(new Vector(node.X, node.Y)))
                        return false;
            }

            return true;
        }

        public static bool IsWalkableTile(Level world, Vector node)
        {
            // Check tileID is walkable
            var tile = world.TileMap[node.X, node.Y];
            if (tile.Ground.Data.IsWalkable && (tile.Object.ID == BlockID.Icicle || tile.Object.ID == BlockID.Root))
                return true;
            return tile.IsWalkable();
        }

        public static bool CanWalkOverInstance(Instance inst)
        {
            return !inst.IsSolid() || inst is Entity || inst is Door;
        }

        private static void DrawPath(Area area, Node lastNode)
        {
            // Trace path back to first node
            while (lastNode.Parent != null)
            {
                area.CrucialNodes.Add(lastNode.ToVector()); // 'Draw' the node
                lastNode = lastNode.Parent;
            }

            area.CrucialNodes.Add(lastNode.ToVector());
        }
    }
}
