using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Spawners
{
    public class Scavenger : Spawner
    {
        public Scavenger(Area area, Vector pos, Random random) : base(area, pos, PickType(SpawnData.ScavengerPopulation, area.Level, random), random)
        {

        }

        public override void Spawn()
        {
            if (!SpawnData.TryGetData(Type, out var data))
                return;

            // Compute the amount of creatures to spawn
            var amtOfCreatures = Random.Next(data.MinLivingAmt, data.MaxLivingAmt + 1);
            amtOfCreatures = Math.Min(amtOfCreatures, MaxAmtSpawnable(Type, Area.Level));
            if (amtOfCreatures < data.MinLivingAmt)
                return;

            int diff = SpawnData.GetDifficulty(Type);
            for (int i = 0; i < amtOfCreatures; i++)
            {
                var inst = Populator.CreateInstanceFromType(Type);

                // Set inst base position
                if (inst is NPC npc) npc.BasePosition = Position;

                // Spawn instance
                if (Area.FindInstanceSpawn(inst, Position, 3, true, false))
                    Area.Level.DifficultyPointsUsed += diff;
            }
        }
    }
}
