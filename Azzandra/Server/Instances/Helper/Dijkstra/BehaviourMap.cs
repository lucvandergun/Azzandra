using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    /// <summary>
    /// Basic map used for Behaviour calculations. Parent class of Dijkstra maps.
    /// Contains a Matrix (2D int array)
    /// </summary>
    public class BehaviourMap
    {
        public Entity Caller { get; protected set; }
        public Level Level { get; protected set; }
        //public int Width { get; private set; }
        //public int Height { get; private set; }
        public float[,] Matrix { get; protected set; }

        public BehaviourMap(Entity caller)
        {
            Level = caller.Level;
            Caller = caller;
        }
    }
}
