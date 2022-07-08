using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class Room : Area
    {
        public int X, Y, W, H;
        public bool IsEnclosed;

        //public List<Vector> EdgeNodesTop = new List<Vector>();
        //public List<Vector> EdgeNodesBottom = new List<Vector>();
        //public List<Vector> EdgeNodesLeft = new List<Vector>();
        //public List<Vector> EdgeNodesRight = new List<Vector>();

        public Room(Level world, int x, int y, int w, int h, bool isEnclosed) : base(world, null)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            IsEnclosed = isEnclosed;

            var list = new List<Vector>(W * H);
            for (int i, j = 0; j < H; j++)
                for (i = 0; i < W; i++)
                    list.Add(new Vector(X + i, Y + j));

            Nodes = list;
        }

        ///// <summary>
        ///// Create a list of nodes located just outside the the nodes of the area.
        ///// </summary>
        //public virtual void IdentifyDirectionalEdgePoints()
        //{
        //    EdgeNodesTop.Clear();
        //    EdgeNodesBottom.Clear();

        //    // Iterate through all points of this area
        //    foreach (var point in Nodes)
        //    {
        //        var adjacent = new List<Vector>(4);
        //        if (point.X > 0) adjacent.Add(new Vector(point.X - 1, point.Y));
        //        if (point.X < 40 - 1) adjacent.Add(new Vector(point.X + 1, point.Y));
        //        if (point.Y > 0) adjacent.Add(new Vector(point.X, point.Y - 1));
        //        if (point.Y < 40 - 1) adjacent.Add(new Vector(point.X, point.Y + 1));

        //        foreach (var adj in adjacent)
        //        {
        //            if (!Nodes.Contains(adj) && !EdgeNodes.Contains(adj))
        //            {
        //                // The point is at the area border -> add to list
        //                EdgeNodes.Add(adj);
        //            }
        //        }
        //    }
        //}

        /*
        public override void IdentifyEdgePoints(int[,] map)
        {
            ListOfEdgePoints.Clear();
            
            // Iterate through all points of this area
            for (int i, j = Math.Max(Y - 1, 1); j < Math.Min(48 - 1, Y + H + 1);)
            {
                for (i = Math.Max(X - 1, 1); i < Math.Min(48 - 1, X + W + 1);)
                {
                    if (i == X - 1 || i == X + W + 1 || j == Y - 1 || j == Y + H + 1)
                        ListOfEdgePoints.Add(new Vector(i, j));
                }
            }
        }
        */
    }
}
