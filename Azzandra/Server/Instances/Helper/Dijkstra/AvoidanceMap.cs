using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AvoidanceMap : BehaviourMap
    {
        public AvoidanceMap(Level level, Entity caller) : base(level, caller)
        {
            
        }

        public void CreateMap()
        {
            // First fill the matrix with -1, then update all fields
            Matrix = new int[Level.MapWidth, Level.MapHeight];
            Matrix.Populate(int.MaxValue);

            var tiles = new List<Vector>();

            if (!(Caller is CarnivorousPlant))
            {
                var avoidRange = new Vector(2);
                int avoidFactor = 5;
                foreach (var inst in Level.ActiveInstances)
                {
                    if (inst is CarnivorousPlant plant)
                    {
                        var nodes = Vector.SumTilesInRange(plant.Position - avoidRange, plant.Size + 2 * avoidRange);
                        Matrix.SetNodes(nodes, avoidFactor);
                    }
                }
            }
            if (!(Caller is GelatinousCube))
            {
                var avoidRange = new Vector(2);
                int avoidFactor = 5;
                foreach (var inst in Level.ActiveInstances)
                {
                    if (inst is GelatinousCube cube)
                    {
                        Matrix.SetNodes(Vector.SumTilesInRange(cube.Position - avoidRange, cube.Size + 2 * avoidRange), avoidFactor);
                    }
                }
            }

            if (!Caller.IsTypeOf(EntityType.Acid))
            {
                int avoidFactor = 2;
                for (int i, j = 0; j < Level.MapHeight; j++)
                {
                    for (i = 0; i < Level.MapWidth; i++)
                    {
                        if (Level.GetTile(i, j).Object.ID == BlockID.Acid)
                        {
                            Matrix[i, j] = avoidFactor;
                        }
                            
                    }
                }
            }
        }
    }
}
