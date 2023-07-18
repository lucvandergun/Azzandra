using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaChest : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            if (area.Level.BenefitPoints < 3)
                return;

            // Find center node:
            var nodes = area.FreeNodes.CreateCopy();
            if (nodes.Count < 1) return;
            nodes.Sort((v1, v2) => v1.OrthogonalLength() - v2.OrthogonalLength());
            var centerPos = nodes[nodes.Count / 2];

            // Spawn chest:
            if (random.NextDouble() > 0.60)
            {
                area.FindInstanceSpawn(new CursedChest(0, 0), centerPos, 1, true, true);
            }
            else
            {
                var tier = Calculator.PickLootTier(area.Level.Depth, random);
                var item = Data.GetDroptableDrop("loot_special", random, tier);
                Chest chest = new LargeChest(0, 0, item);
                if (area.FindInstanceSpawn(chest, centerPos, 1, true, true))
                {
                    area.Level.LevelManager.RemoveBenefit(3);
                }
            }
        }
    }
}
