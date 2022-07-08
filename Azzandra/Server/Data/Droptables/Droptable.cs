using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    [JsonConverter(typeof(DroptableConverter))]
    public class Droptable
    {
        public DroptableEntry[] Table;

        public Item[] GetDrop(Random random, int tier = -1)
        {
            if (Table == null || Table.Length <= 0)
                return null;

            // Convert entries to tuple with weight value up front
            var list = Table.Select(e => Tuple.Create(e.Weight, e));

            // Remove entries of incorrect tier only if correct tier is required, tiers of -1 are always allowed
            if (tier != -1)
            {
                list = list.Where(e => e.Item2.Tier == tier || e.Item2.Tier == -1);
            }

            // Skip next step if list is empty by any chance
            if (list.Count() <= 0)
                return new Item[0];

            // Pick and return a random entry
            return Util.PickItemByWeight(list, random).GetDrop(random, tier);
        }


        // Static forward-method to get droptable drop from table-id.
        public static Item[] RollDrop(string droptableID, Random random, int tier = -1)
        {
            return Data.GetDroptableDrop(droptableID, random, tier);
        }
    }
}
