using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public abstract class Spawner
    {
        public Area Area { get; protected set; }
        public Vector Position { get; protected set; }
        public Type Type { get; protected set; }
        public Random Random { get; set; }

        public Spawner(Area area, Vector pos, Type type, Random random)
        {
            Area = area;
            Position = pos;
            Type = type;
            Random = random;
        }

        /// <summary>
        /// Spawns whatever this spawner does. Must be overridden.
        /// </summary>
        public abstract void Spawn();

        /// <summary>
        /// Picks a random entity type based on a list of entity type potentials.
        /// </summary>
        public static Type PickType(List<Tuple<int, Type>> entityPotentials, Level level, Random random)
        {
            // Compile sublist of entity potentials able/allowed to spawn in current world
            var list = entityPotentials.Where(e => AllowedSpawn(e.Item2, level)).ToList();
            if (list.Count <= 0)
                return null;

            // Pick single type
            return Util.PickItemByWeight(list, random);
        }

        /// <summary>
        /// Computes the max amount of entities spawned based on the remaining difficulty points remaining of the level.
        /// </summary>
        public static int MaxAmtSpawnable(Type entityType, Level level)
        {
            int diff = SpawnData.GetDifficulty(entityType);
            int maxAmt = level.GetDifficultyPointsRemaining / diff;
            return maxAmt;
        }

        /// <summary>
        /// Checks whether certain entity type can spawn according to its depth and temperature occurrences.
        /// </summary>
        public static bool AllowedSpawn(Type type, Level level)
        {
            return type == null ? false : SpawnData.GetData(type)?.CanSpawn(level.Depth, level.Temp) ?? false;
        }
    }
}
