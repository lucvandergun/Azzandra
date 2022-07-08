using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaShrine : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            //if (area.Level.BenefitPoints < 4)
            //    return;

            AddSpawners = false;

            // Find center node:
            //var nodes = area.FreeNodes.CreateCopy();
            //if (nodes.Count < 1) return;
            //nodes.Sort((v1, v2) => v1.OrthogonalLength() - v2.OrthogonalLength());
            //var centerPos = nodes[nodes.Count / 2];

            // Spawn shrine:
            var shrine = new Shrine(0, 0);
            if (area.FindInstanceSpawn(shrine, area.RemoteNode, 3, true, true))
            {
                //area.Level.LevelManager.RemoveBenefit(4);
                //new BlobBrush(BlockID.Plant, false, false, false, 3).Paint(area.Level, shrine.Position, random);
            }

            
        }
    }
}
