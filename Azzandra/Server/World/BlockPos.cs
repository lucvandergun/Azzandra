using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct BlockPos
    {
        public Vector Position { get; private set; }
        public bool IsFloor { get; private set; }

        public BlockPos(Vector pos, bool isFloor)
        {
            Position = pos;
            IsFloor = isFloor;
        }
    }
}
