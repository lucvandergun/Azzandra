using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class LakeData
    {
        //public readonly Temp Min, Max;
        public readonly int Type;
        public readonly int Density;
        public LakeData(int typeID, int density)
        {
            Type = typeID;
            Density = density;
        }
    }
}
