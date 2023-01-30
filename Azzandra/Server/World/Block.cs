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
        public int Value;
        
        public Block(int id)
        {
            ID = id;
            Value = 0;
        }

        public Block(int id, int value)
        {
            ID = id;
            Value = value;
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
