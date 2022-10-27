using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class PopulatorBossLevel : Populator
    {
        //protected override int CalculateDifficultyPoints(int depth) => 30 + (int)(depth * 7.5f);

        protected Area BossRoom;

        public PopulatorBossLevel(Level level) : base(level)
        {

        }

        protected override void PopulateAreas()
        {
            // Populate the dungeon
            PaintDungeon();

            // Spawn Boss
            var maxSize = Areas.Max(a => a.Size);
            BossRoom = Areas.Find(a => a.Size == maxSize);
            if (!BossRoom.FindInstanceSpawn(new Azzandra(0, 0), true))
                Level.LevelManager.Server.ThrowError("Could not spawn boss!");
            
            Spawners.Shuffle();
            foreach (var s in Spawners)
                s.Spawn();
        }

        protected override void AssignStartAndEnd()
        {
            // Compile candidates: smallest room first (99% there is only one room but still)
            var areas = GetNonPathwayAreas();
            areas = areas.OrderBy(a => a.Size);

            // Spawns only the stairs up! Level end position is set to be the same as the start just to be sure.
            foreach (var area in areas)
            {
                Start = area;
                End = area;
                area.EdgeNodes.Shuffle(Random);

                var stairsUp = new StairsUp(0, 0);
                foreach (var node in area.EdgeNodes)
                {
                    if (FindStairsLocation(Start, stairsUp)) //(Start.FindInstanceSpawn(stairsUp, node, 5, true, true))
                    {
                        StartPosition = stairsUp.Position;
                        EndPosition = stairsUp.Position;
                        return;
                    }
                }
            }
        }

        protected override void PaintDungeon()
        {
            //Paint(new BlobBrush(BlockID.Mud, true, false, true, 9), 5);
            Paint(new ScatterBrush(BlockID.Rock, false, true, true, 3), 5);
            Paint(new ScatterBrush(BlockID.Crystal, false, true, true, 2), 4);
        }
    }
}
