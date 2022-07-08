using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Roomgenerators
{
    public class RoomCrafting : IRoomGenerator
    {
        public void PopulateRoom(Area area, Random random)
        {
            int amtOfChests = area.World.Random.Next(3) + 1;
            area.FreeNodes.Shuffle(random);

            var chest = new Chest(0, 0);

            for (int i = area.FreeNodes.Count() - 1; i >= 0; i--)
            {
                var node = area.FreeNodes[i];
                chest.Position = node;
                
                if (area.TrySpawnInstance(chest, true))
                {
                    chest = new Chest(0, 0);
                    amtOfChests--;
                    if (amtOfChests <= 0)
                    {
                        break;
                    }
                }
            }
        }
    }
}
