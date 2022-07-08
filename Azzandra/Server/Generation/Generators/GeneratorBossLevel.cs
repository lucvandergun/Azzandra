using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class GeneratorBossLevel : Generator
    {
        protected override void AssignPopulator(Level level) => new PopulatorBossLevel(level).PopulateLevel();

        public GeneratorBossLevel(Level level) : base(level)
        {
            MapWidth = 32;
            MapHeight = 32;

            CavePercent = 65;
            LakeIncorporationPercent = 100;

            RoomMinAmount = 0;
            RoomMaxAmount = 0;
        }

        protected override void GenerateAreaLayout()
        {
            var temp = Level.Temp;

            // Create cavern tilemap:
            IDMap = CreateAreaPattern(MapWidth, MapHeight, CavePercent, CaveSmoothness);
            IDMap = ChangeAreas(IDMap, BlockID.Floor, BlockID.Wall, x => x.Nodes.Count() < MinCaveSize);

            // Enlarge map and add lakes:
            IDMap = ExpandMapBorder(IDMap, 4);
            var lakeData = GetLakeTypes(temp);
            IDMap = GenerateLakes(IDMap, lakeData, LakeIncorporationPercent, MinLakeSize, MinCaveSize);

            Areas = IdentifyAreas(IDMap, BlockID.Floor);

            // Shuffle areas and identify edge points:
            Areas.ForEach(a => a.IdentifyEdgePoints());
            Areas.Shuffle(Random);

            // Create connections and pathways between all areas:
            ConnectAreas(IDMap, Areas);
        }

        protected override List<LakeData> GetLakeTypes(Temp temp)
        {
            var list = new List<LakeData>(2);
            list.Add(new LakeData(BlockID.Lava, 56));
            return list;
        }
    }
}
