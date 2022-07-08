using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Spawners
{
    public class Graves : Spawner
    {
        public Graves(Area area, Vector pos, Random random) : base(area, pos, typeof(Wraith), random)
        {

        }
        
        public override void Spawn()
        {
            //if (!SpawnData.TryGetData(Type, out var data))
            //    return;

            //// Compute the amount of creatures to spawn
            //var amtOfCreatures = Random.Next(data.MinLivingAmt, data.MaxLivingAmt + 1);
            //amtOfCreatures = Math.Min(amtOfCreatures, MaxAmtSpawnable(Type, Area.Level));
            //if (amtOfCreatures < 2 * data.MinLivingAmt)
            //    return;

            // Spawn graves
            int amtOfGraves = 4 + Random.Next(4);
            for (int i = 0; i < amtOfGraves; i++)
            {
                Area.FindInstanceSpawn(new Grave(0, 0), Position, 3, true, false);
            }

            // Spawn Necromancer
            int diff = SpawnData.GetDifficulty(typeof(Necromancer));
            var inst = new Necromancer(0, 0);
            inst.BasePosition = Position;
            if (Area.FindInstanceSpawn(inst, Position, 3, true, false))
                Area.Level.DifficultyPointsUsed += diff;
        }
    }
}
