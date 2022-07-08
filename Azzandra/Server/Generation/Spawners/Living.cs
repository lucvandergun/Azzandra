using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Spawners
{
    public class Living : Spawner
    {
        public Living(Area area, Vector pos, Random random) : base(area, pos, PickType(SpawnData.LivingPopulation, area.Level, random), random)
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

            if (Type == typeof(VineBlight) || Type == typeof(CarnivorousPlant))
            {
                new ScatterBrush(BlockID.Plant, false, false, false, 4, 1).Paint(Area.Level, Position, Random);
            }
            else if (Type == typeof(Necromancer))
            {
                // Spawn graves
                int amtOfGraves = 4 + Random.Next(4);
                for (int i = 0; i < amtOfGraves; i++)
                {
                    Area.FindInstanceSpawn(new Grave(0, 0), Position, 3, true, false);
                }
            }

            // Spawn wolves:
            if (Type == typeof(Wolf))
            {
                SpawnWolves(amtOfCreatures);
                return;
            }

            int diff = SpawnData.GetDifficulty(Type);
            for (int i = 0; i < amtOfCreatures; i++)
            {
                var type = (i == 0 && Type == typeof(Wolf)) ? typeof(AlphaWolf) : Type;
                var inst = Populator.CreateInstanceFromType(type);

                // Set inst base position
                if (inst is NPC npc)
                    npc.BasePosition = Position;

                // Spawn instance
                if (Area.FindInstanceSpawn(inst, Position, 2, true, false))
                    Area.Level.DifficultyPointsUsed += diff;
            }
        }

        private void SpawnWolves(int amtOfCreatures)
        {
            int diff = SpawnData.GetDifficulty(Type);
            var aw = new AlphaWolf(0, 0);

            for (int i = 0; i < amtOfCreatures; i++)
            {
                var inst = i == 0 ? aw : Populator.CreateInstanceFromType(Type);

                // Set inst base position
                if (inst is NPC npc)
                    npc.BasePosition = Position;

                // Spawn instance
                if (Area.FindInstanceSpawn(inst, Position, 2, true, false))
                {
                    Area.Level.DifficultyPointsUsed += diff;
                    if (i != 0)
                    {
                        inst.Parent = new InstRef(aw);
                        aw.Children.Add(new InstRef(inst));
                    }
                }  
            }
        }
    }
}
