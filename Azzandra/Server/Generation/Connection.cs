using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class Connection
    {
        public Area A1, A2;
        public List<Vector> Nodes;
        public bool HasDoor = false;

        public Connection(Area a1, Area a2, List<Vector> nodes)
        {
            A1 = a1;
            A2 = a2;
            Nodes = nodes;
        }

        public Connection(Area a1, Area a2, Vector node)
        {
            A1 = a1;
            A2 = a2;
            Nodes = new List<Vector> { node };
        }

        /// <summary>
        /// Gets the other area as specified from one of the present connecting areas.
        /// </summary>
        /// <param name="a">The area not to return.</param>
        /// <returns>The other area present in the connection.</returns>
        public Area GetOther(Area a)
        {
            return a == A1 ? A2 : A1;
        }

        /// <summary>
        /// Retrieves the very first node of the list of connecting nodes. If none are present, vector zero is returned.
        /// </summary>
        /// <returns>The very first node in the list of connecting nodes.</returns>
        public Vector GetNode()
        {
            return Nodes == null ? Vector.Zero : Nodes.Count() > 0 ? Nodes[0] : Vector.Zero;
        }
    }
}
