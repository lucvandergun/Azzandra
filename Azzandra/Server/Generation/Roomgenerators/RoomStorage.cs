using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Roomgenerators
{
    public class RoomStorage : IRoomGenerator
    {
        public void PopulateRoom(Area area, Random random)
        {
            int chests, barrels;
            chests = random.Next(1, 4);
            barrels = random.Next(2, 6);

            area.FreeNodes.Shuffle(random);

            // Spawn chests
            for (int i = 0; i < chests; i++)
            {
                area.FindInstanceSpawn(new Chest(0, 0), true);
            }

            // Spawn barrels
            for (int i = 0; i < barrels; i++)
            {
                area.FindInstanceSpawn(new Barrel(0, 0), true);
            }

            // Spawn scavengers
            if (random.Next(100) < 50)
            {
                new SpawnerScavenger().Populate(area, random);
            } 
        }
    }
}
