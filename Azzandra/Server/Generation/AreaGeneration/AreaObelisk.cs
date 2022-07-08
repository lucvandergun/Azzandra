using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class AreaObelisk : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            if (area.Level.BenefitPoints < 4)
                return;

            // Find center node:
            var nodes = area.FreeNodes.CreateCopy();
            if (nodes.Count < 1) return;
            nodes.Sort((v1, v2) => v1.OrthogonalLength() - v2.OrthogonalLength());
            var centerPos = nodes[nodes.Count / 2];

            // Spawn chest:
            var obelisk = new Obelisk(0, 0);
            if (area.FindInstanceSpawn(obelisk, centerPos, 1, true, true))
            {
                area.Level.LevelManager.RemoveBenefit(4);
            }
        }
    }
}
