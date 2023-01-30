using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Spawners
{
    public class Haunted : Spawner
    {
        public Haunted(Area area, Vector pos, Random random) : base(area, pos, PickType(SpawnData.HauntedPopulation, area.Level, random), random)
        {

        }

        public override void Spawn()
        {
            if (Type == null)      // 'enemyType' can be null, especially in the earlier levels, when there are no valid creature types to spawn yet.
                return;

            if (MaxAmtSpawnable(Type, Area.Level) <= 0)  // Chech whether the level difficulty points still allow a spawn.
                return;

            var inst = (NPC)Populator.CreateInstanceFromType(Type);
            if (inst == null) return;

            inst.IsHaunting = true;
            if (Area.FindInstanceSpawn(inst, false))
                Area.Level.DifficultyPointsUsed += SpawnData.GetDifficulty(Type);

            //Area.Level.Server.ThrowDebug("Spawned a haunting " + Type.Name + ".");
        }
    }
}
