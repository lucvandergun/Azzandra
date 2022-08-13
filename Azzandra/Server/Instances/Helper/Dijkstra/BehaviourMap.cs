using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class BehaviourMap
    {
        public Entity Caller { get; protected set; }
        public Level Level { get; protected set; }
        //public int Width { get; private set; }
        //public int Height { get; private set; }
        public int[,] Matrix { get; protected set; }

        public BehaviourMap(Level level, Entity caller)
        {
            Level = level;
            Caller = caller;
        }
    }
}
