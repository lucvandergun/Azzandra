using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaCamp : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            if (area.Level.BenefitPoints < 3)
                return;
            
            var pos = area.RemoteNode;
            var type = Spawner.PickType(SpawnData.CampPopulation, area.Level, random);
            if (!SpawnData.TryGetData(type, out var data))
                return;

            // Compute the amount of creatures to spawn
            var amtOfCreatures = random.Next(data.MinLivingAmt + 1, (int)Math.Ceiling(1.5f * data.MaxLivingAmt) + 1);

            // Create campfire instance:
            Campfire fire = null;
            if (type == typeof(Goblin) || type == typeof(Hobgoblin))
            {
                var item = Data.GetDroptableDrop("loot_stew", random, Calculator.PickLootTier(area.Level.Depth, random));
                fire = new Campfire(0, 0, item.First());
            }
            else if (type == typeof(Troll))
                fire = new Campfire(0, 0, Item.Create("troll_stew"));

            // Find campfire/camp location: all neigbouring tiles are free, and in total 16 tiles free in the first two layers.
            var potentials = area.FreeNodes.Where(fn => area.FreeNodes.Where(fn2 => (fn2 - fn).ChebyshevLength() == 1).Count() >= 8
                && area.FreeNodes.Where(fn2 => (fn2 - fn).ChebyshevLength() == 2).Count() >= 8).ToList();
            potentials.Sort((a, b) => (a - area.RemoteNode).ChebyshevLength() - (b - area.RemoteNode).ChebyshevLength());
            foreach (var p in potentials)
            {
                fire.Position = p;
                if (area.TrySpawnInstance(fire, true))
                    break;
            }
            if (fire.Position == Vector.Zero)
                return;
            pos = fire.Position;

            int diff = SpawnData.GetDifficulty(type);
            for (int i = 0; i < amtOfCreatures; i++)
            {
                var inst = Populator.CreateInstanceFromType(type);

                // Set inst base position
                if (inst is NPC npc)
                    npc.BasePosition = pos;

                // Spawn instance
                if (area.FindInstanceSpawn(inst, pos, 3, true, false))
                    area.Level.DifficultyPointsUsed += diff;
            }

            area.EventLocations.Add(fire.Position);
            area.Level.RemoveBenefit(3);
        }
    }
}
