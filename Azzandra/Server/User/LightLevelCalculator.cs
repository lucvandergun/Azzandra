using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class LightLevelCalculator
    {
        public const int MAX_STRENGTH = 8;

        /// <summary>
        /// Creates a lightmap of MAX_STRENGTH decaying until a certain reach.
        /// </summary>
        /// <param name="reach">The reach of the lightsource (includes origin).</param>
        /// <returns>2D float array of size 2 * reach - 1.</returns>
        public static float[,] CalculateVisibleLight(Level level, Vector origin, int reach)
        {
            return CalculateLightMap(level, origin, MAX_STRENGTH, reach);
        }

        /// <summary>
        /// Creates a lightmap of the given strength. (This is automatically the reach as well.)
        /// </summary>
        /// <param name="strength">The strength AND reach of the lightsource (includes origin).</param>
        /// <returns>2D float array of size 2 * strength - 1.</returns>
        public static float[,] EmitLight(Level level, Vector origin, int strength)
        {
            return CalculateLightMap(level, origin, strength, strength);
        }

        /// <summary>
        /// Calculates the light levels as emitted with a specified strength, and puts only that area in a 2d array.
        /// </summary>
        /// <param name="strength">The initial strength of the lightsource (light level).</param>
        /// <param name="reach">The reach of the lightsource (includes origin).</param>
        /// <returns>2D float array of size reach * 2 - 1.</returns>
        private static float[,] CalculateLightMap(Level level, Vector absoluteOrigin, int strength, int reach)
        {
            float orthDecay, diagDecay;
            orthDecay = (float)strength / (float)reach;
            diagDecay = (float)Math.Sqrt(2d * orthDecay * orthDecay);

            // Create submatrix
            int w, h;
            w = reach * 2 - 1;
            h = w;
            var map = new float[w, h];

            var absoluteOffset = absoluteOrigin - new Vector(reach);

            // Set initial light
            var center = new Vector(reach);
            map[center.X, center.Y] = strength;

            // Breadth-first algorithm:
            var undone = new List<Vector>() { center };
            var done = new List<Vector>();
            while (undone.Count > 0)
            {
                var current = undone.First();
                undone.RemoveAt(0);
                done.Add(current);

                if (level.NodeBlocksLight(absoluteOffset + current))
                    continue;

                // Identify, update & possibly add neighbours
                UpdateAdjacent(level, absoluteOffset, map, current, done, undone, orthDecay, diagDecay);
            }

            return map;
        }

        private static void UpdateAdjacent(Level level, Vector absoluteOffset, float[,] map, Vector relativeOffset, List<Vector> done, List<Vector> undone, float orthDecay, float diagDecay)
        {
            // All bounds are inclusive and relative to the given relative pos!
            int x1, y1, x2, y2;
            x1 = Math.Max(0, relativeOffset.X - 1);
            y1 = Math.Max(0, relativeOffset.Y - 1);
            x2 = Math.Min(relativeOffset.X + 1, map.GetLength(0) - 1);
            y2 = Math.Min(relativeOffset.Y + 1, map.GetLength(1) - 1);

            for (int i, j = y1; j <= y2; j++)
            {
                for (i = x1; i <= x2; i++)
                {
                    // Set decay based on whether diagonal or not: (at corners of 3x3 area is diagonal, else orthogonal)
                    float decay = ((i == x1 || i == x2) && (j == y1 || j == y2)) ? diagDecay : orthDecay;
                    var newLight = map[relativeOffset.X, relativeOffset.Y] - decay;

                    var pos = new Vector(i, j);

                    if (map[pos.X, pos.Y] < newLight)
                    {
                        map[pos.X, pos.Y] = newLight;    // Update light if brighter
                        if (!done.Contains(pos) && !undone.Contains(pos))   // Add to undone list if not handled yet
                        {
                            undone.Add(pos);
                        }
                    }
                }
            }
            
            //var ortho = new List<Vector>() { new Vector(0, 1), new Vector(0, -1), new Vector(1, 0), new Vector(-1, 0) };
            //UpdateTiles(ortho, orthDecay, from, map, done, undone);
            //var diag = new List<Vector>() { new Vector(1, 1), new Vector(1, -1), new Vector(-1, 1), new Vector(-1, -1) };
            //UpdateTiles(diag, diagDecay, from, map, done, undone);
        }

        /// <summary>
        /// Adds a lightmap on top of an existing lightmap, with its center at the given origin.
        /// </summary>
        /// <param name="baseMap"></param>
        /// <param name="origin"></param>
        /// <param name="newMap"></param>
        public static void AddLight(this float[,] baseMap, Vector origin, float[,] newMap)
        {
            // Calculate dimensions of the overlay map, and the top-left coordinates to placed at, according to the origin.
            int nw, nh;
            nw = newMap.GetLength(0);
            nh = newMap.GetLength(1);

            int xoff, yoff;
            xoff = origin.X - nw / 2 - 1;
            yoff = origin.Y - nh / 2 - 1;

            // Calculate the intersecting region of both maps (coordinates are of main map).
            int x1, y1, x2, y2;
            x1 = Math.Max(0, -xoff);
            y1 = Math.Max(0, -yoff);
            x2 = Math.Min(nw, baseMap.GetLength(0) - xoff);
            y2 = Math.Min(nh, baseMap.GetLength(1) - yoff);

            // Overlay the new map:
            for (int i, j = y1; j < y2; j++)   // i and j are in the new map
            {
                for (i = x1; i < x2; i++)
                {
                    var newLight = newMap[i, j];
                    if (newLight > baseMap[i + xoff, j + yoff])
                        baseMap[i + xoff, j + yoff] = newLight;
                }
            }
        }

        //private void UpdateTiles(List<Vector> dirs, float decay, Vector from, float[,] map, List<Vector> done, List<Vector> undone)
        //{
        //    // Rather do this: float for loop, decay dependent on whether corner or not. Check does light block, check out of map bounds
        //    // Add to real map: foreach: if higher, update
        //    // Also: larger origins
            
        //    // Check all for their light level: is it lower than possible? update & add if not yet added
        //    foreach (var p in dirs)
        //    {
        //        var pos = from + p;

        //        // Check in bounds:
        //        var newLight = map[from.X, from.Y] - decay;
        //        if (map[pos.X, pos.Y] < newLight)
        //        {
        //            map[pos.X, pos.Y] = newLight;    // Update light if brighter
        //            if (!done.Contains(pos) && !undone.Contains(pos))   // Add to undone list if not handled yet
        //            {
        //                undone.Add(pos);
        //            }
        //        }

        //    }
        //}

    }
}
