using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public abstract class AreaGenerator
    {
        public bool AddSpawners = true;
        public abstract void PopulateArea(Area area, Random random);
    }
}
