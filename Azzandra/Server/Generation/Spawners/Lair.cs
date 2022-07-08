using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Spawners
{
    public class Lair : Spawner
    {
        public Lair(Area area, Vector pos, Random random) : base(area, pos, PickType(SpawnData.LairPopulation, area.Level, random), random)
        {

        }
        
        public override void Spawn()
        {
            if (!SpawnData.TryGetData(Type, out var data))
                return;

            // Compute the amount of creatures to spawn
            var amtOfCreatures = Random.Next(data.MinLivingAmt + 1, (int)Math.Ceiling(1.5f * data.MaxLivingAmt) + 1);
            amtOfCreatures = Math.Min(amtOfCreatures, MaxAmtSpawnable(Type, Area.Level));
            if (amtOfCreatures < data.MinLivingAmt + 1)
                return;

            // Place tiles based on instance type
            if (Type == typeof(Goblin) || Type == typeof(Hobgoblin))
            {
                var item = Data.GetDroptableDrop("loot_stew", Random, Calculator.PickLootTier(Area.Level.Depth, Random));
                Area.FindInstanceSpawn(new Campfire(0, 0, item.First()), Position, 0, true, true);
                //for (int i = 0; i < amtOfCreatures / 2; i++)
                //    Area.FindTileSpawn(BlockID.Torch, false, Position, 3, false, true);
            }
            else if (Type == typeof(ToxicSpider))
            {
                new ScatterBrush(BlockID.Cobweb, false, false, true, 3).Paint(Area.Level, Position, Random);
            }

            int diff = SpawnData.GetDifficulty(Type);
            for (int i = 0; i < amtOfCreatures; i++)
            {
                var inst = Populator.CreateInstanceFromType(Type);
                
                // Set inst base position
                if (inst is NPC npc)
                    npc.BasePosition = Position;

                // Spawn instance
                if (Area.FindInstanceSpawn(inst, Position, 3, true, false))
                    Area.Level.DifficultyPointsUsed += diff;
            }
        }
    }
}
