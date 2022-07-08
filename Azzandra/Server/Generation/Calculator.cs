using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public static class Calculator
    {
        /// <summary>
        /// Picks a loot tier (int) based on the current depth.
        /// Occurrences:
        ///  Tier 1:  1 til 13   (100% from 1 til 6)
        ///  Tier 2:  7 til 25   (100% from 14 til 18)
        ///  Tier 3: 19 til 30   (100% from 26 til 30)
        /// </summary>
        /// <returns>int, either 1, 2 or 3</returns>
        public static int PickLootTier(int depth, Random random)
        {
            float end1, start2, end2, start3, border;
            end1 = 6;
            start2 = 14;
            end2 = 18;
            start3 = 26;
            border = 8;
            
            float p1, p2;
            p1 = depth <= end1 ? 1 : 
                depth > end1 && depth < start2 ? (border - depth + end1) / border : 
                0;
            p2 = depth > end1 && depth < start2 ? (depth - end1) / border :
                depth >= start2 && depth <= end2 ? 1 : 
                depth > end2 && depth < start3 ? (border - depth + end2) / border : 
                0;
            //p3 = depth > end2 && depth < start3 ? (depth - end2) / border :
            //    depth >= start3 ? 1 :
            //    0;

            float roll = (float)random.NextDouble();
            int tier = roll < p1 ?
                1 : roll < p1 + p2 ?
                2 :
                3;
            return tier;
        }
    }
}
