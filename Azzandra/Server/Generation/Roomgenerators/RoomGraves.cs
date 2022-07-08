using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.Roomgenerators
{
    public class RoomGraves : IRoomGenerator
    {
        public void PopulateRoom(Area area, Random random)
        {
            // Random grave placement
            //if (random.Next(2) == 0)
            {
                int amtOfGraves = Math.Min(area.World.Random.Next(5) + 5, area.Nodes.Count() / 6);
                area.FreeNodes.Shuffle(random);

                for (int i = 0; i < amtOfGraves; i++)
                {
                    area.FindInstanceSpawn(new Grave(0, 0), false);
                }
            }
            // Row grave placement
            //else
            //{
            //    int amtOfGraves = Math.Min(area.World.Random.Next(5) + 5, area.Nodes.Count() / 6);
            //    int graveAttempts = amtOfGraves * 2;

            //    area.FreeNodes.Shuffle(random);
            //    var rootNode = area.FreeNodes.PickRandom();
            //    var node = rootNode;
            //    int maxX = rootNode.X + 7;

            //    for (int i = 0; i < graveAttempts && amtOfGraves > 0; i++)
            //    {
            //        if (area.TrySpawnInstance(new Grave(node.X, node.Y)))
            //        {
            //            amtOfGraves--;
            //            node.X += 2;
            //        }
            //        else
            //        {
            //            node = new Vector(rootNode.X + random.Next(5) - 2, rootNode.Y + 2);
            //        }

            //    }
            //}
            
        }
    }
}
