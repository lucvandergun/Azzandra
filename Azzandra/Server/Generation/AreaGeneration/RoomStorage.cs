using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class RoomStorage : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            int maxChests = 3, maxBoxes = 3;

            var centerPos = area.FreeNodes.PickRandom(random);

            // Spawn chests
            var tables = new List<string>() { "loot_weapons", "loot_equipment" };
            int tableIndex = random.Next(tables.Count);
            for (int i = 0; i < maxChests; i++)
            {
                if (i >= 2 && random.NextDouble() > 0.25d)
                    break;

                if (area.Level.BenefitPoints < 3)
                    break;

                var table = tables[tableIndex];
                var item = Droptable.RollDrop(table, random, Calculator.PickLootTier(area.Level.Depth, random));
                //var item = LootGenerator.PickChestLoot(area.Level.LevelManager, area.Level.Depth, random);
                if (item != null && item.Length > 0)
                {
                    var chest = new Chest(0, 0, item);
                    if (area.FindInstanceSpawn(chest, centerPos, 3, true, true))
                    {
                        area.Level.LevelManager.RemoveBenefit(3);
                        tableIndex = (tableIndex + 1) % tables.Count;
                    }
                }
            }

            // Spawn barrels
            for (int i = 0; i < maxBoxes; i++)
            {
                if (random.NextDouble() > (maxBoxes - i) / maxBoxes)
                    break;

                if (area.Level.BenefitPoints < 1)
                    break;

                var item = LootGenerator.GetLoot("loot_barrel", area.Level.Depth, random);
                if (item != null && item.Length > 0)
                {
                    var barrel = new AmmunitionBarrel(0, 0, item);
                    if (area.FindInstanceSpawn(barrel, centerPos, 3, true, true))
                    {
                        area.Level.LevelManager.RemoveBenefit(2);
                    }
                }
            }

            // Spawn scavengers
            if (random.NextDouble() < 0.8d)
            {
                new Spawners.Scavenger(area, centerPos, random).Spawn();
                AddSpawners = false;
            }
        }
    }
}
