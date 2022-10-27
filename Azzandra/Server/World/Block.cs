using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct Block : IEquatable<Block>
    {
        public int ID;
        
        public Block(int id)
        {
            ID = id;
        }

        public BlockData Data => BlockID.GetData(ID);

        public override string ToString()
        {
            return ID.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Block block)
                return this.Equals(block);

            return false;
        }
        public bool Equals(Block block)
        {
            return block.ID == ID;
        }

        public static bool operator ==(Block block1, Block block2) => block1.Equals(block2);
        public static bool operator !=(Block block1, Block block2) => !block1.Equals(block2);
    }
}
