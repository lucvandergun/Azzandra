using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public static class LootGenerator
    {
        public static Item[] PickChestLoot(LevelManager lm, int depth, Random random)
        {
            var tier = Calculator.PickLootTier(depth, random);
            var drop = Droptable.RollDrop("loot_chest", random, tier);
            return drop;
        }

        public static Item[] GetLoot(string table, int depth, Random random)
        {
            var tier = Calculator.PickLootTier(depth, random);
            var drop = Droptable.RollDrop(table, random, tier);
            return drop;
        }
    }
}
