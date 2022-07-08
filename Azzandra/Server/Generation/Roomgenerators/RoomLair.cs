using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Roomgenerators
{
    public class RoomLair : IRoomGenerator
    {
        public void PopulateRoom(Area area, Random random)
        {
            area.FreeNodes.Shuffle(random);

            new SpawnerLair().Populate(area, random);
        }
    }
}
