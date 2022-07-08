using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public interface IBrush
    {
        int Paint(Level level, Vector start, Random random);
        int GetTileType();
    }

    public abstract class Brush : IBrush
    {
        public readonly int TileType;
        public int Strength;
        public readonly bool IsFloor, CheckObstruction, RemoveNonSolidObjectNodes;

        public Brush(int tileID, bool isFloor, bool checkObstruction, bool removeNonSolidObjectNodes, int strength)
        {
            TileType = tileID;
            IsFloor = isFloor;
            CheckObstruction = checkObstruction;
            RemoveNonSolidObjectNodes = removeNonSolidObjectNodes;
            Strength = strength;
        }

        public int GetTileType() => TileType;

        public abstract int Paint(Level level, Vector start, Random random);
    }
}
