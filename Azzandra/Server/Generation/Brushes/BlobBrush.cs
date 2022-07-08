using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class BlobBrush : Brush
    {
        public BlobBrush(int tileID, bool isFloor, bool checkObstruction, bool removeNonSolidObjectNodes, int strength) : base(tileID, isFloor, checkObstruction, removeNonSolidObjectNodes, strength)
        { }

        public override int Paint(Level level, Vector start, Random random)
        {

            // Create blob pattern
            int w = Strength + 2, h = Strength + 2;
            var blob = CreateBlobPattern(w, h, random);
            int amt = 0;

            // Overlay blob on level map
            for (int i, j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                {
                    if (blob[i, j] == 1)
                        continue;

                    var pos = start + new Vector(i - w / 2, j - h / 2);

                    var area = level.GetArea(pos.X, pos.Y);
                    if (area != null) 
                    {
                        if (area.TryCreateTile(pos, TileType, IsFloor, CheckObstruction, RemoveNonSolidObjectNodes))
                        {
                            amt++;

                            //// Just to be sure: Remove area free node regardless of walkable for object tiles:
                            //if (!IsFloor) area.FreeNodes.Remove(pos);
                        }
                    }
                }
            }

            return amt;
        }


        /// <summary>
        /// Creates a blob of max size w x h, min size w/2, h/2.
        /// </summary>
        /// <param name="w">The max width</param>
        /// <param name="h">The max height</param>
        /// <returns>A 2d array of booleans of size w x h</returns>
        public static int[,] CreateBlobPattern(int w, int h, Random random)
        {
            var arr = new int[w, h];
            arr = Generator.RandomFillMap(arr, 45, random);

            // Fill grid: inner area is true by default, outer area is random.
            for (int i, j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                {
                    if (i >= 0.25f * w && i < 0.75f * w && j >= 0.25f * h && j < 0.75f * h) // inner area
                        arr[i, j] = 0;
                    else
                        arr[i, j] = 50 > random.Next(1, 101) ? 0 : 1; // 50 % chance of being true
                }
            }

            // Smoothify grid
            for (int amt = 0; amt < 1; amt++)
            {
                arr = Generator.SmoothifyMap(arr);
            }

            return arr;
        }
    }
}
