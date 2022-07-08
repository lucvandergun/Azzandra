using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct Block
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
    }
}
