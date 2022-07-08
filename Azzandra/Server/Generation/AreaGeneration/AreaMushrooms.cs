using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaMushrooms : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            int small, large;
            small = random.Next(area.Size / 10, area.Size / 6);
            large = random.Next(area.Size / 16, area.Size / 12);

            // Spawn large
            for (int i = 0; i < large; i++)
            {
                var mush = new GiantMushroom(0, 0);
                var pos = area.FreeNodes.PickRandom(random);
                area.FindInstanceSpawn(mush, pos, 9, true, false);
            }

            // Spawn small
            for (int i = 0; i < small; i++)
            {
                var mush = BlockID.LightMushroom;
                var pos = area.FreeNodes.PickRandom(random);
                area.FindTileSpawn(mush, false, pos, 9, true, true);
            }
        }
    }
}
