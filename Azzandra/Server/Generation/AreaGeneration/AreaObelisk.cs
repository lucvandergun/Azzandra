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

            // Spawn obelisk:
            var obelisk = new Obelisk(0, 0);
            if (area.FindInstanceSpawn(obelisk, area.RemoteNode, 3, true, true))
            {
                area.Level.LevelManager.RemoveBenefit(4);
                //new BlobBrush(BlockID.Plant, false, false, false, 3).Paint(area.Level, obelisk.Position, random);
            }
        }
    }
}
