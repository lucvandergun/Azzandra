using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaNothing : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            // Pass - do absolutely nothing.
        }
    }
}
