using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation.AreaGeneration
{
    public class RoomLibrary : AreaGenerator
    {
        public override void PopulateArea(Area area, Random random)
        {
            AddSpawners = false;

            if (!(area is Room room))
                return;

            int min = (room.W + room.H) / 3, max = (room.W + room.H);
            int amtBookcases = random.Next(max - min) + min;
            double bookChance = 0.25d;
            int spellBenefit = 4;

            var borderNodes = room.FreeNodes.Where(f => room.EdgeNodes.Any(e => (e - f).OrthogonalLength() == 1)).ToList();
            borderNodes.Shuffle(random);

            int amtSpells = 0, maxSpells = 2;
            while (borderNodes.Count > 0 && amtBookcases > 0)
            {
                //bool hasSpell = amtSpells < maxSpells && (amtSpells == 0 || random.NextDouble() <= bookChance && room.Level.LevelManager.BenefitValue >= spellBenefit);
                bool hasSpell = amtSpells == 0;

                var node = borderNodes[0];
                borderNodes.RemoveAt(0);

                var bookcase = new Bookcase(node.X, node.Y, hasSpell);
                if (room.TrySpawnInstance(bookcase, true))
                {
                    room.Level.LevelManager.RemoveBenefit(spellBenefit);
                    amtBookcases--;
                    amtSpells++;
                }
            }
        }
    }
}
