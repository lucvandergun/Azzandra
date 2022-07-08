using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class ConnectionPotential
    {
        public Area BaseArea, NewArea;
        public Vector BasePoint, NewPoint;

        public virtual int Distance => (BasePoint - NewPoint).OrthogonalLength();
        
        public ConnectionPotential(Area baseArea, Area newArea, Vector basePoint, Vector newPoint)
        {
            BaseArea = baseArea;
            NewArea = newArea;
            BasePoint = basePoint;
            NewPoint = newPoint;
        }
    }

    public class ConnectionPotentialTouching : ConnectionPotential
    {
        public override int Distance => 0;

        public ConnectionPotentialTouching(Area baseArea, Area newArea, Vector basePoint, Vector newPoint) : base(baseArea, newArea, basePoint, newPoint)
        {

        }
    }
}
