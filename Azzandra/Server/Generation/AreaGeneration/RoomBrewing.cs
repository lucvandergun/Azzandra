using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class RoomBrewing : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            var pos = area.FreeNodes.PickRandom(random);
            int benefit = 3;

            bool hasSpawnedTankard = false;
            bool hasSpawnedReward = false;

            // Spawn items (barrel or table)
            for (int i = 0; i < 3; i++)
            {
                if (i != 0 && random.NextDouble() < 0.8)
                    break;
                
                if (area.Level.BenefitPoints < benefit)
                    break;

                if (random.Next(2) > 0)
                {
                    if (SpawnBarrel(area, random, pos, true))
                    {
                        area.Level.LevelManager.RemoveBenefit(benefit);
                        hasSpawnedReward = true;

                        if (!hasSpawnedTankard && random.Next(10) > 0)
                        {
                            hasSpawnedTankard = area.FindInstanceSpawn(new GroundItem(0, 0, Item.Create("tankard")), true);
                        }
                    }  
                } 
                else
                {
                    if (SpawnPotion(area, random, pos, true))
                    {
                        area.Level.LevelManager.RemoveBenefit(benefit);
                        hasSpawnedReward = true;
                    }
                }
            }

            if (!hasSpawnedReward)
                return;

            for (int i = 0; i < 1 + random.Next(3); i++)
            {
                if (random.Next(2) > 0)
                    SpawnBarrel(area, random, pos, false);
                else
                    SpawnPotion(area, random, pos, false);
            }
        }

        private bool SpawnBarrel(Area area, Random random, Vector pos, bool reward)
        {
            var type = reward ? DrinkData.IDs[random.Next(DrinkData.IDs.Length)] : null;
            var barrel = new Barrel(0, 0, type);
            if (area.FindInstanceSpawn(barrel, pos, 4, true, true))
            {
                return true;
            }
            return false;
        }

        private bool SpawnPotion(Area area, Random random, Vector pos, bool reward)
        {
            if (area.FindTileSpawn(BlockID.Table, false, pos, 4, true, true, out var tablePos))
            {
                if (reward)
                {
                    var tier = Calculator.PickLootTier(area.Level.Depth, random);
                    var items = Droptable.RollDrop("potions", random, tier);
                    foreach (var item in items)
                    {
                        area.Level.CreateInstance(new GroundItem(tablePos.X, tablePos.Y, item));
                    }
                }

                // Spawn stools:
                int amt = 0;
                var dirs = Vector.Dirs8;
                dirs.Shuffle(random);
                foreach (var dir in dirs)
                {
                    area.FindTileSpawn(BlockID.Stool, false, tablePos, 2, false, true);
                    amt++;
                    if (amt >= 4) break;
                }

                return true;
            }
            
            return false;
        }
    }
}
