using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class ScatterBrush : Brush
    {
        private int Spread;
        
        public ScatterBrush(int tileID, bool isFloor, bool checkObstruction, bool removeNonSolidObjectNodes, int strength, int spread = 2) : base(tileID, isFloor, checkObstruction, removeNonSolidObjectNodes, strength)
        {
            Spread = spread;
        }

        public override int Paint(Level level, Vector start, Random random)
        {
            bool isSolid = !BlockID.GetData(TileType).IsWalkable;

            var adjacent = Vector.Dirs9;
            if (Spread >= 2)
            {
                adjacent = adjacent.InnerSum().Distinct().ToList();
            }
            Paint(level, random, Strength, start, isSolid, adjacent);

            return -1;
        }

        private void Paint(Level level, Random random, int strength, Vector pos, bool isSolid, List<Vector> adjacent)
        {
            if (!level.IsInMapBounds(pos.X, pos.Y))
                return;
            
            // Check whether tile can be placed on node - forget node if not
            int areaID = level.AreaReferences[pos.X, pos.Y];
            var area = level.GetAreaFromID(areaID);

            if (area == null || (level.TileMap[pos.X, pos.Y].Ground.ID != BlockID.Floor && level.TileMap[pos.X, pos.Y].Ground.ID != BlockID.Mud))
                return;
            else if (isSolid && !area.FreeNodes.Contains(pos))
                return;

            // Try to create tile - stop here if not possible
            else if (!area.TryCreateTile(pos, TileType, IsFloor, CheckObstruction, RemoveNonSolidObjectNodes))
                return;

            adjacent.Shuffle();
            for (int i = 0; i < strength; i++)
            {
                Paint(level, random, strength - 1, pos + adjacent[i], isSolid, adjacent);
            }
        }
    }
}
