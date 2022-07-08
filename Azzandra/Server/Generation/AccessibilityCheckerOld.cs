using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public static class AccessibilityCheckerOld
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

            public Vector ToVector() => new Vector(X, Y);

            public int OrthogonalDistanceTo(Node other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
            }
        }


        /// <summary>
        /// Calculates a list of nodes from start to target destination.
        /// Returns whether the target is reached.
        /// </summary>
        public static bool IsAccessible(Level level, Area area, Vector _start, Vector _target, List<Vector> obstructingTiles, bool draw = false)
        {
            Node start = new Node(_start.X, _start.Y);
            Node target = new Node(_target.X, _target.Y);

            var areaNodes = area.Nodes; // DEBUG: change to FreeNodes? and elaborate walkable tiles check for full reliability
            if (obstructingTiles == null) obstructingTiles = new List<Vector>();

            //declare start node
            start.H = ComputeHScore(start.X, start.Y, target.X, target.Y);
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
                // Get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // Add current node to the closed list & remove it from the open list
                closedList.Add(current);
                openList.Remove(current);

                // Check if target was reached: (at most one tile difference!)
                if (closedList.FirstOrDefault(l => l.OrthogonalDistanceTo(target) <= 1) != null)
                {
                    if (draw) DrawPath(area, current);
                    return true;
                }

                var adjacentSquares = GetWalkableAdjacentSquares(level, area, current, target, areaNodes, obstructingTiles);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // If this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(n => n.X == adjacentSquare.X
                            && n.Y == adjacentSquare.Y) != null)
                        continue;

                    // If it's not in the open list...
                    if (openList.FirstOrDefault(n => n.X == adjacentSquare.X && n.Y == adjacentSquare.Y) == null)
                    {
                        // Compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, _target.X, _target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // And add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // Test if using the current G score makes the adjacent square's F score
                        // Lower, if yes update the parent because it means it's a better path
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
                    return false;
                }
            }

            // At this point the A* has failed to find a valid path

            if (draw)
            {
                level.Server.ThrowError("Accessibility check failed: " + _start + " to " + _target + ", Area: " + area.ID);
                DrawPath(area, current);
            }
            return false;
        }


        private static int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }


        private static List<Node> GetWalkableAdjacentSquares(Level world, Area area, Node node, Node target, List<Vector> areaNodes, List<Vector> obstructingTiles)
        {
            // Get potential directions to move in:
            var potentialDirs = new List<Dir>() { new Dir(0, 1), new Dir(0, -1), new Dir(1, 0), new Dir(-1, 0) };
            var nodes = new List<Node>();

            // Try each:
            foreach (var dir in potentialDirs)
            {
                var newNode = new Node(node.X + dir.X, node.Y + dir.Y);

                // If area's nodes contains node or node equals target, and node is not one of the obstructing tiles
                if ((areaNodes.Contains(newNode.ToVector()) || (newNode.X == target.X && newNode.Y == target.Y)) && !obstructingTiles.Contains(newNode.ToVector()))
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
            var tile = world.TileMap[node.X, node.Y];
            if (!tile.IsWalkable())
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
            if (!tile.IsWalkable())
                return false;

            return true;
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
