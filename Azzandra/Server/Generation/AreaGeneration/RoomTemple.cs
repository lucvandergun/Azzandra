using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class RoomTemple : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            AddSpawners = false;

            if (!(area is Room room))
                return;

            // Calculate the inner direction of the room
            bool horizontal = room.W > room.H;
            int side = random.Next(1) == 0 ? 1 : -1;
            var dir = horizontal ? new Dir(side, 0) : new Dir(0, side);

            // Pick random altar position
            var pots = new List<Vector>();
            if (horizontal)
            {
                pots = area.FreeNodes.Where(f => area.EdgeNodes.Any(e => (e - f).X == side * 2 && e.Y == f.Y)).ToList();
            }
            else
            {
                pots = area.FreeNodes.Where(f => area.EdgeNodes.Any(e => (e - f).Y == side * 2 && e.X == f.X)).ToList();
            }

            if (pots.Count < 1) return;
            pots.Sort((v1, v2) => v1.OrthogonalLength() - v2.OrthogonalLength());
            var altarNode = pots.Count % 2 == 0
                ? pots[pots.Count / 2 - (Util.Random.Next(2) == 1 ? 1 : 0)]
                : pots[pots.Count / 2];

            var altar = new Altar(0, 0);
            if (!area.FindInstanceSpawn(altar, altarNode, 0, true, true))
            {
                return;
            }

            altarNode = altar.Position;

            // Spawn torches
            if (!horizontal)
            {
                area.TryCreateTile(altarNode - new Vector(1, 0), BlockID.Torch, false, true);
                area.TryCreateTile(altarNode + new Vector(1, 0), BlockID.Torch, false, true);

                area.TryCreateTile(altarNode - new Vector(-2, side * 2), BlockID.Bench, false, true);
                area.TryCreateTile(altarNode - new Vector(-1, side * 2), BlockID.Bench, false, true);
                area.TryCreateTile(altarNode - new Vector(1, side * 2), BlockID.Bench, false, true);
                area.TryCreateTile(altarNode - new Vector(2, side * 2), BlockID.Bench, false, true);
            }
            else
            {
                area.TryCreateTile(altarNode - new Vector(0, 1), BlockID.Torch, false, true);
                area.TryCreateTile(altarNode + new Vector(0, 1), BlockID.Torch, false, true);
            }
        }
    }
}
