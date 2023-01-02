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

        public void AddPillars(Random random)
        {
            // Spawn pillars prior to populating it
            //if (W <= 4 || H <= 4)
            //    return;

            // Calculate the inner direction of the room
            bool horizontal = W > H;
            int breadth = horizontal ? H : W;
            int length = horizontal ? W : H;
            int dist = 1;// random.Next(Math.Min(2, breadth - 3));

            if (breadth <= 4 || length % 2 == 0)
                return;

            bool even = length % 2 == 0;
            int offset = 1;// even ? 0: random.Next(1);
            int effectiveLength = length - 2 * offset;
            int step = 2;// (effectiveLength - 1) % 2 == 0 ? 2 : (effectiveLength - 1) % 3 == 0 ? 3 : effectiveLength - 1;

            Vector startNode = new Vector(X, Y);
            int x, x2, y, y2;
            //int add = even ? 0 : 2;
            for (int j = offset; j < length - offset; j += step) //j < (length - offset - 1) / 2 + add
            {
                x = horizontal ? j : dist;
                x2 = horizontal ? j : breadth - dist - 1;
                y = horizontal ? dist : j;
                y2 = horizontal ? breadth - dist - 1 : j;
                //var opposite = horizontal ? new Vector(length - 2 * j - offset - 1, 0) : new Vector(0, length - 2 * j - offset - 1);
                TryCreateTile(startNode + new Vector(x, y), BlockID.Pillar, false, true, true, true);
                TryCreateTile(startNode + new Vector(x2, y2), BlockID.Pillar, false, true, true, true);
                //TryCreateTile(startNode + new Vector(x, y) + opposite, BlockID.Pillar, false, true);
                //TryCreateTile(startNode + new Vector(x2, y2) + opposite, BlockID.Pillar, false, true);
            }
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
