using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaData
    {
        public static Dictionary<Type, AreaData> DataDictionary = new Dictionary<Type, AreaData>()
        {
            { typeof(RoomStorage),  new AreaData(2, 1, 99, Temp.Glacial, Temp.Scorching) },
            { typeof(RoomBrewing),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching) },
            { typeof(RoomTemple),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching) },
            { typeof(RoomLibrary),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching, a => a.Size <= 40 && a is Room r && r.IsEnclosed) },
            { typeof(AreaChest),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching, a => !a.IsStart && !a.IsEnd && a.Size <= 50 && a.Connections.Count == 1) },
            { typeof(AreaShrine),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching, a => a.Size <= 30) },
            { typeof(AreaObelisk),  new AreaData(1, 1, 99, Temp.Glacial, Temp.Scorching) },
            { typeof(AreaMushrooms),  new AreaData(99, 12, 18, Temp.Lukewarm, Temp.Warm, a => a.Size >= 40) },
            { typeof(AreaCamp),  new AreaData(2, 1, 99, Temp.Glacial, Temp.Scorching, a => a.Size >= 40) },
        };

        public readonly int LowestDepth, UpperDepth;
        public readonly Temp LowestTemp, UpperTemp;
        public readonly int MaxAmt;
        public readonly Func<Area, bool> Predicates;

        public AreaData(int maxAmt, int lowestDepth, int upperDepth, Temp lowestTemp, Temp upperTemp, Func<Area, bool> predicates = null)
        {
            MaxAmt = maxAmt;
            LowestDepth = lowestDepth;
            UpperDepth = upperDepth;
            LowestTemp = lowestTemp;
            UpperTemp = upperTemp;
            Predicates = predicates;
        }

        /// <summary>
        /// Checks whether this entity type can occur/spawn at the given area, depth and temperature.
        /// </summary>
        public bool CanGenerate(Area area, int depth, Temp temp, int occurrences)
        {
            return
                depth >= LowestDepth && depth <= UpperDepth
                && (int)temp >= (int)LowestTemp && (int)temp <= (int)UpperTemp
                && occurrences < MaxAmt
                && (Predicates?.Invoke(area) ?? true);
        }

        public static AreaData GetData(Type genType)
        {
            return DataDictionary.TryGetValue(genType, out var data) ? data : null;
        }
        public static bool TryGetData(Type genType, out AreaData data)
        {
            return DataDictionary.TryGetValue(genType, out data);
        }


        // === Primary Generation Types === \\
        public static readonly List<Tuple<int, Type>> CavernPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create<int, Type>(12, null),
            Tuple.Create(6, typeof(AreaChest)),
            Tuple.Create(6, typeof(AreaCamp)),
            //Tuple.Create(3, typeof(AreaObelisk)),
        };

        public static readonly List<Tuple<int, Type>> RoomPopulation = new List<Tuple<int, Type>>
        {
            Tuple.Create<int, Type>(12, null),
            Tuple.Create(12, typeof(RoomStorage)),
            Tuple.Create(6, typeof(RoomBrewing)),
            Tuple.Create(9, typeof(RoomLibrary)),
            Tuple.Create(6, typeof(AreaChest)),
            Tuple.Create(6, typeof(AreaCamp)),
            //Tuple.Create(3, typeof(AreaObelisk)),
        };


        // === Secondary Generation Types === \\
        public static readonly List<Tuple<int, Type>> CavernPopulation2 = new List<Tuple<int, Type>>
        {
            Tuple.Create<int, Type>(90, null),
            //Tuple.Create(10, typeof(AreaMushrooms)),
        };

        public static readonly List<Tuple<int, Type>> RoomPopulation2 = new List<Tuple<int, Type>>
        {
            Tuple.Create<int, Type>(12, null),
        };


        public static Type PickPrimaryAreaType(Area area, int depth, Temp temp, List<Type> occurrences, Random random)
        {
            if (area is Pathway) return null;

            // Select potentials, filter by possibility and pick single type at random by weight.
            var potentials = area is Room ? RoomPopulation : CavernPopulation;
            var filtered = potentials.Where(p => p.Item2 == null || GetData(p.Item2).CanGenerate(area, depth, temp, occurrences.Count(t => t == p.Item2)));

            // Make sure there no empty rooms at dead ends
            if (area.Connections.Count == 1 && !filtered.All(f => f.Item2 == null))
                filtered = filtered.Where(f => f.Item2 != null);

            return Util.PickItemByWeight(filtered, random);
        }

        public static Type PickSecondaryAreaType(Area area, int depth, Temp temp, Random random)
        {
            if (area is Pathway) return null;

            // Select potentials, filter by possibility and pick single type at random by weight.
            var potentials = area is Room ? RoomPopulation2 : CavernPopulation2;
            var filtered = potentials.Where(p => p.Item2 == null || GetData(p.Item2).CanGenerate(area, depth, temp, 0));

            return Util.PickItemByWeight(filtered, random);
        }
    }
}
