using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class VisibilityCalculator
    {

        private Entity CallingInst;
        private MyVisibility Visibility;
        public List<Vector> VisibleTiles;

        public VisibilityCalculator(Entity callingInst)
        {
            CallingInst = callingInst;
            
            Visibility = new MyVisibility(
                BlocksLight,
                SetVisible,
                GetDistance
                );

            VisibleTiles = new List<Vector>(16);
        }

        /// <summary>
        /// Calculates whether the target instance is visible as measured from the calling inst (no sight obstructions).
        /// </summary>
        /// <param name="targetInst"></param>
        /// <returns></returns>
        public bool IsInstanceVisible(Instance targetInst)
        {
            //if (CallingInst.IsCollisionWith(targetInst))
            //    return true;
            
            // Boundize vision range by the maximum distance the target is from the calling instance: worth it?
            var maxTargetDistance = CallingInst.TileDistanceTo(targetInst) + targetInst.Size.ChebyshevLength() + CallingInst.Size.ChebyshevLength() - 2;
            var visionRange = Math.Min(CallingInst.GetVisionRange(), maxTargetDistance);

            return AreAnyTilesVisible(targetInst.GetTiles(), visionRange);
        }

        public bool IsTileVisible(Vector node)
        {
            // Boundize vision range by the maximum distance the target is from the calling instance: worth it?
            var maxTargetDistance = CallingInst.DistanceTo(node).ChebyshevLength() + CallingInst.Size.ChebyshevLength() - 1;
            var visionRange = Math.Min(CallingInst.GetVisionRange(), maxTargetDistance);

            return AreAnyTilesVisible(new List<Vector>() { node }, visionRange);
        }

        public bool AreAnyTilesVisible(IEnumerable<Vector> nodes, int visionRange)
        {
            var origin = CallingInst.Position;

            foreach (var octant in CalculateOctants(origin, nodes))
            {
                Visibility.ComputeOctant(CallingInst.Position, octant, visionRange);
            }

            return AreAnyOnVisibleTile(nodes);
        }

        /// <summary>
        /// Returns true if the instance is on top of at least one visible tile.
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        private bool AreAnyOnVisibleTile(IEnumerable<Vector> nodes)
        {
            //// Return false if the calling instance is outside the map bounds (either top left or bottom right) 
            //if (!CallingInst.Level.IsInMapBounds(inst.X, inst.Y) || !CallingInst.Level.IsInMapBounds(inst.X + inst.GetW() - 1, inst.Y + inst.GetH() - 1))
            //    return false;

            return nodes.Any(t => VisibleTiles.Contains(t));
        }

        /// <summary>
        /// Retuns a list of all octants that the other instance is in as measured form the origin point.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="inst"></param>
        /// <returns></returns>
        private List<int> CalculateOctants(Vector origin, IEnumerable<Vector> nodes)
        {
            // Loop through all other instance's tiles
            var octants = new List<int>();

            foreach (var node in nodes)
            {
                var octant = CalculateOctant(origin, node);
                if (!octants.Contains(octant))
                    octants.Add(octant);
            }

            return octants;
        }

        /// <summary>
        /// Calculates the corresponding octant the target tile is in compared to the origin point.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private int CalculateOctant(Vector origin, Vector target)
        {
            var dist = target - origin;
            int x = dist.X, y = dist.Y;

            int AX = origin.X, AY = origin.Y, BX = target.X, BY = target.Y;

            int octant;

            // right
            if (BX > AX)
            {
                // down
                if (BY > AY)
                {
                    if ((BY - AY) < (BX - AX))
                        octant = 7;
                    else
                        octant = 6;
                }
                // up
                else
                {
                    if ((AY - BY) < (BX - AX))
                        octant = 0;
                    else
                        octant = 1;
                }
            }
            // left
            else
            {
                // down
                if (BY > AY)
                {
                    if ((BY - AY) < (AX - BX))
                        octant = 4;
                    else
                        octant = 5;
                }
                // up
                else
                {
                    if ((AY - BY) < (AX - BX))
                        octant = 3;
                    else
                        octant = 2;
                }
            }
            return octant;
        }


        private bool BlocksLight(int x, int y)
        {
            var world = CallingInst.Level;
            var tile = world.GetTile(x, y);

            // For now only ground tiles are checked! This is because there aren't any opaque object tiles
            if (!CallingInst.CanSeeThroughBlock(tile.Ground))
                return true;

            // Check whether instance blocks light:
            var pos = new Vector(x, y);
            foreach (var inst in world.ActiveInstances)
            {
                if (inst.BlocksLight())
                {
                    if (inst.GetTiles().Contains(pos))
                        return true;
                }
            }

            return false;
        }

        private void SetVisible(int x, int y)
        {
            VisibleTiles.Add(new Vector(x, y));
        }

        public int GetDistance(int x, int y)
        {
            return new Vector(x, y).EuclidianLength();
        }
    }
}
