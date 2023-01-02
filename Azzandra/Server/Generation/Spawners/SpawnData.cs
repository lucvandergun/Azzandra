using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class SpawnData
    {
        public static Dictionary<Type, SpawnData> DataDictionary = new Dictionary<Type, SpawnData>()
        {
            { typeof(Goblin),       new SpawnData(1, 2, 3, 1, 24, Temp.Freezing, Temp.Warm) },
            { typeof(Kobold),       new SpawnData(1, 1, 2, 1, 12, Temp.Freezing, Temp.Warm) },
            { typeof(Fiend),        new SpawnData(2, 1, 2, 16, 29, Temp.Warm, Temp.Scorching) },
            { typeof(Hobgoblin),    new SpawnData(1, 2, 3, 17, 29, Temp.Cold, Temp.Hot) },
            { typeof(Skeleton),     new SpawnData(1, 1, 2, 1, 14, Temp.Glacial, Temp.Scorching) },
            { typeof(ArmouredSkeleton),new SpawnData(3, 1, 2, 15, 29, Temp.Glacial, Temp.Scorching) },
            { typeof(Wraith),       new SpawnData(5, 1, 1, 6, 29, Temp.Glacial, Temp.Scorching) },
            { typeof(RockGolem),    new SpawnData(7, 1, 1, 5, 15, Temp.Cold, Temp.Scorching) },
            { typeof(CrystalGolem), new SpawnData(8, 1, 1, 19, 29, Temp.Cold, Temp.Scorching) },
            { typeof(Zombie),       new SpawnData(10, 1, 1, 7, 20, Temp.Freezing, Temp.Hot) },

            { typeof(IceElemental), new SpawnData(1, 1, 2, 3, 9, Temp.Glacial, Temp.Cold) },
            { typeof(IceGiant),     new SpawnData(15, 1, 1, 6, 11, Temp.Glacial, Temp.Cold) },
            { typeof(Wolf),         new SpawnData(2, 3, 4, 1, 7, Temp.Freezing, Temp.Lukewarm) },
            { typeof(Ghast),        new SpawnData(6, 1, 2, 5, 10, Temp.Cold, Temp.Hot) },
            { typeof(Gargoyle),     new SpawnData(12, 1, 1, 2, 12, Temp.Cold, Temp.Hot) },
            { typeof(ShadowElemental),new SpawnData(3, 1, 1, 3, 14, Temp.Glacial, Temp.Scorching) },
            { typeof(Ghaugh),       new SpawnData(5, 2, 3, 8, 13, Temp.Glacial, Temp.Cold) },
            { typeof(CaveWorm),     new SpawnData(3, 1, 2, 1, 7, Temp.Cold, Temp.Hot) },
            { typeof(LostMage),     new SpawnData(2, 1, 1, 1, 6, Temp.Cold, Temp.Hot) },

            { typeof(VampireBat),   new SpawnData(4, 1, 3, 16, 22, Temp.Lukewarm, Temp.Hot) },
            { typeof(GelatinousCube),new SpawnData(14, 1, 2, 10, 16, Temp.Cold, Temp.Warm) },
            { typeof(Spectre),      new SpawnData(9, 1, 1, 13, 21, Temp.Lukewarm, Temp.Warm) },
            { typeof(VineBlight),   new SpawnData(8, 1, 2, 11, 17, Temp.Lukewarm, Temp.Warm) },
            { typeof(ToxicSpider),  new SpawnData(2, 3, 5, 12, 19, Temp.Lukewarm, Temp.Warm) },
            { typeof(WolfSpider),   new SpawnData(12, 1, 1, 15, 21, Temp.Lukewarm, Temp.Warm) },
            { typeof(CarnivorousPlant),new SpawnData(9, 1, 2, 13, 19, Temp.Lukewarm, Temp.Hot) },
            { typeof(Troll),        new SpawnData(11, 1, 1, 9, 15, Temp.Freezing, Temp.Hot) },
            { typeof(Necromancer),  new SpawnData(8, 1, 1, 8, 19, Temp.Freezing, Temp.Hot) },
            { typeof(ShadowHound),  new SpawnData(4, 2, 3, 14, 21, Temp.Cold, Temp.Hot) },
            { typeof(Ooze),         new SpawnData(4, 1, 3, 17, 22, Temp.Lukewarm, Temp.Hot) },

            { typeof(Demon),        new SpawnData(15, 1, 1, 25, 29, Temp.Hot, Temp.Scorching) },
            { typeof(FireElemental),new SpawnData(2, 1, 2, 23, 29, Temp.Hot, Temp.Scorching) },
            { typeof(FireGiant),    new SpawnData(13, 1, 1, 24, 29, Temp.Hot, Temp.Scorching) },
            { typeof(Hellhound),    new SpawnData(8, 2, 3, 22, 29, Temp.Hot, Temp.Scorching) },
            { typeof(Cockatrice),   new SpawnData(8, 1, 2, 21, 29, Temp.Lukewarm, Temp.Hot) },
            { typeof(Choker),       new SpawnData(10, 1, 2, 23, 29, Temp.Lukewarm, Temp.Scorching) },
            { typeof(Enchanter),    new SpawnData(10, 1, 1, 20, 29, Temp.Cold, Temp.Scorching) },
            { typeof(DustElemental),   new SpawnData(8, 1, 2, 18, 27, Temp.Warm, Temp.Scorching) },
        };

        public readonly int LowestDepth, UpperDepth;
        public readonly Temp LowestTemp, UpperTemp;
        public readonly int Weight, Difficulty, MinLivingAmt, MaxLivingAmt;

        public SpawnData(int difficulty, int minLivingAmt, int maxLivingAmt, int lowestDepth, int upperDepth, Temp lowestTemp, Temp upperTemp)
        {
            //Weight = weight;
            Difficulty = difficulty;
            MinLivingAmt = minLivingAmt;
            MaxLivingAmt = maxLivingAmt;
            LowestDepth = lowestDepth;
            UpperDepth = upperDepth;
            LowestTemp = lowestTemp;
            UpperTemp = upperTemp;
        }

        /// <summary>
        /// Checks whether this entity type can occur/spawn at the given depth and temperature.
        /// </summary>
        /// <param name="depth">The given depth of the level.</param>
        /// <param name="temp">The given temperature of the level.</param>
        /// <returns>Whether the entity type belonging to this spawn data can be spawned.</returns>
        public bool CanSpawn(int depth, Temp temp)
        {
            return
                depth >= LowestDepth && depth <= UpperDepth &&
                (int)temp >= (int)LowestTemp && (int)temp <= (int)UpperTemp;
        }

        public static SpawnData GetData(Type entityType)
        {
            if (entityType == null)
                return null;
            return DataDictionary.TryGetValue(entityType, out var data) ? data : null;
        }
        public static bool TryGetData(Type entityType, out SpawnData data)
        {
            if (entityType == null)
            {
                data = null;
                return false;
            }
            return DataDictionary.TryGetValue(entityType, out data);
        }

        public static int GetDifficulty(Type entityType)
        {
            return DataDictionary.TryGetValue(entityType, out var data) ? data.Difficulty : 10;
        }


        // == Populations ==\\

        public static readonly List<Tuple<int, Type>> LairPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create(2, typeof(Skeleton)),
            //Tuple.Create(4, typeof(Goblin)),
            Tuple.Create(3, typeof(Wolf)),
            //Tuple.Create(3, typeof(Hobgoblin)),
            //Tuple.Create(3, typeof(ToxicSpider)),
            Tuple.Create(3, typeof(Hellhound)),
            Tuple.Create(3, typeof(ShadowHound)),
        };

        public static readonly List<Tuple<int, Type>> CampPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create(4, typeof(Goblin)),
            Tuple.Create(3, typeof(Hobgoblin)),
            Tuple.Create(3, typeof(Troll)),
        };

        public static readonly List<Tuple<int, Type>> ScavengerPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create(4, typeof(Goblin)),
            Tuple.Create(4, typeof(Hobgoblin)),
        };

        public static readonly List<Tuple<int, Type>> LivingPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create(2, typeof(Skeleton)),
            Tuple.Create(2, typeof(ArmouredSkeleton)),
            Tuple.Create(1, typeof(Goblin)),
            Tuple.Create(1, typeof(Hobgoblin)),
            Tuple.Create(2, typeof(RockGolem)),
            Tuple.Create(2, typeof(CrystalGolem)),

            Tuple.Create(3, typeof(Wolf)),
            Tuple.Create(3, typeof(IceElemental)),
            Tuple.Create(2, typeof(IceGiant)),
            Tuple.Create(1, typeof(Gargoyle)),
            Tuple.Create(3, typeof(Ghaugh)),
            Tuple.Create(2, typeof(CaveWorm)),
            Tuple.Create(2, typeof(LostMage)),
            //Tuple.Create(1, typeof(Ghast)),

            Tuple.Create(1, typeof(Necromancer)),
            Tuple.Create(2, typeof(CarnivorousPlant)),
            Tuple.Create(3, typeof(VampireBat)),
            Tuple.Create(2, typeof(WolfSpider)),
            Tuple.Create(3, typeof(ToxicSpider)),
            Tuple.Create(2, typeof(VineBlight)),
            //Tuple.Create(3, typeof(Troll)),
            Tuple.Create(3, typeof(ShadowHound)),
            Tuple.Create(3, typeof(Ooze)),
            Tuple.Create(2, typeof(GelatinousCube)),

            Tuple.Create(3, typeof(FireElemental)),
            Tuple.Create(2, typeof(Cockatrice)),
            Tuple.Create(3, typeof(Hellhound)),
            Tuple.Create(2, typeof(FireGiant)),
            Tuple.Create(2, typeof(Demon)),
            Tuple.Create(2, typeof(Choker)),
            Tuple.Create(2, typeof(Enchanter)),
            Tuple.Create(3, typeof(DustElemental)),
        };

        public static readonly List<Tuple<int, Type>> HauntedPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create(4, typeof(Kobold)),
            Tuple.Create(2, typeof(Fiend)),
            Tuple.Create(2, typeof(Wraith)),
            Tuple.Create(2, typeof(Spectre)),
            //Tuple.Create(3, typeof(ShadowElemental)),
            Tuple.Create(1, typeof(Zombie)),
            //Tuple.Create(1, typeof(Ghast)),
        };
    }
}
