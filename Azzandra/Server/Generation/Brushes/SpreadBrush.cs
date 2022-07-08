using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class SpreadBrush : Brush
    {
        public SpreadBrush(int tileID, bool isFloor, bool checkObstruction, bool removeNonSolidObjectNodes, int strength) : base(tileID, isFloor, checkObstruction, removeNonSolidObjectNodes, strength)
        { }

        public override int Paint(Level level, Vector start, Random random)
        {
            bool isSolid = !BlockID.GetData(TileType).IsWalkable;
            var undone = new List<Vector>();
            var visited = new List<Vector>();
            undone.Add(start);

            var adjacent = new Vector[]
                { new Vector(1, 0), new Vector(-1, 0), new Vector(0, 1), new Vector(0, -1) };

            while (undone.Count() > 0)
            {
                // Adress current node
                var current = undone.Last();
                undone.Remove(current);
                visited.Add(current);

                // Check whether tile can be placed on node - forget node if not
                int areaID = level.AreaReferences[current.X, current.Y];
                var area = level.GetAreaFromID(areaID);

                if (area == null || (level.TileMap[current.X, current.Y].Ground.ID != BlockID.Floor && level.TileMap[current.X, current.Y].Ground.ID != BlockID.Mud))
                    continue;

                else if (isSolid && !area.FreeNodes.Contains(current))
                    continue;

                else if (!area.TryCreateTile(current, TileType, IsFloor, CheckObstruction, RemoveNonSolidObjectNodes))
                    continue;

                if (!IsFloor) // Remove free node from area if is object tile
                    area.FreeNodes.Remove(current);

                // Add four new undone nodes
                foreach (var dir in adjacent)
                {
                    var newNode = current + dir;
                    if (!visited.Contains(newNode))
                        undone.Add(newNode);
                }

                undone.Shuffle();

                // End loop condition
                var delta = current - start;
                Strength -= delta.EuclidianLength();
                if (Strength <= 0)
                    break;
            }

            return visited.Count();
        }
    }
}
