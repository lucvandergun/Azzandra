using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class VisibilityHandler
    {
        private readonly Server Server;
        private readonly Visibility Visibility;

        public List<Instance> VisibleInstances { get; private set; }
        public List<Instance> VisibleEnvironmentInstances { get; private set; }
        public bool[,] VisibilityMap { get; private set; }  // The currently visible tiles: not the memory tilemap!


        public VisibilityHandler(Server server)
        {
            VisibleInstances = new List<Instance>();

            Server = server;
            Visibility = new MyVisibility(
                BlocksLight, 
                SetVisible,
                GetDistance
                );
        }

        public void SetupVisibilityMap(Level world)
        {
            int w = world != null ? world.MapWidth : 0;
            int h = world != null ? world.MapHeight : 0;
            VisibilityMap = new bool[w, h];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="sightRadius"></param>
        public void Update(Vector origin, int sightRadius = 8)
        {
            var level = Server.LevelManager.CurrentLevel;
            var player = Server.User.Player;

            // Update current visibility map & instances:
            VisibilityMap.Populate(false);
            Visibility.Compute(origin, sightRadius);
            VisibleInstances = level.ActiveInstances.Where(i => IsInstanceVisible(i)).ToList();
            VisibleEnvironmentInstances = VisibleInstances.Where(i => i.CanBeTargetedByPlayer()).ToList(); //.Where(i => i is Entity || i is GroundItem).ToList();
            VisibleEnvironmentInstances.Remove(Server.User.Player);
            VisibleEnvironmentInstances.Sort((a, b) => a.TileDistanceTo(player) - b.TileDistanceTo(player));

            // Update light map by player vision:
            //LightMap.Populate(0f);
            var lightMap = Server.LevelManager.CurrentLevel.StaticLightMap.CreateCopy();
            lightMap.AddLight(origin, LightLevelCalculator.CalculateVisibleLight(level, origin, 8));

            // (Round light levels) and set upper bound
            for (int i, j = 0; j < lightMap.GetLength(1); j++)
            {
                for (i = 0; i < lightMap.GetLength(0); i++)
                {
                    var light = lightMap[i, j]; //(float)Math.Round((double)lightMap[i, j]); 
                    lightMap[i, j] = Math.Min(light, LightLevelCalculator.MAX_STRENGTH);
                }
            }

            Server.LevelManager.CurrentLevel.LightMap = lightMap;

            // Update memory tile map:
            Server.LevelManager.CurrentLevel.UpdateMemoryTileMap(VisibilityMap);
        }

        public Tile GetMemoryTile(int x, int y)
        {
            return Server.LevelManager.CurrentLevel.GetMemoryTile(x, y);
        }
        public bool IsTileVisible(int x, int y)
        {
            if (Server.LevelManager.CurrentLevel.IsInMapBounds(x, y))
                return VisibilityMap[x, y];
            else
                return false;
        }

        public bool IsInstanceVisible(Instance inst)
        {
            // The player is always visible to the user
            if (inst is Player)
                return true;

            // Don't bother calculating if the instance is outide of the map bounds
            if (!Server.LevelManager.CurrentLevel.IsInMapBounds(inst.X, inst.Y) || !Server.LevelManager.CurrentLevel.IsInMapBounds(inst.X + inst.GetW() - 1, inst.Y + inst.GetH() - 1))
                return true;

            // For an instance to be visible two conditions must be met:
            //  1. Any tile under it must be in an unobstructed sightline from the player.
            //  2. Such specific tile must also have a light level greater than zero.
            foreach (var tile in inst.GetTiles())
                if (VisibilityMap[tile.X, tile.Y] && Server.LevelManager.CurrentLevel.GetTileLightness(tile) > 0)
                    return true;
            
            return false;
        }


        public bool BlocksLight(int x, int y)
        {
            // TODO: Unify with Combatant.CanSeeThroughTile()!
            var level = Server.LevelManager.CurrentLevel;

            // Check whether tile blocks light:
            var tileID = level.GetTile(x, y);
            switch (tileID.Ground.ID)
            {
                case BlockID.Wall:
                case BlockID.Pillar:
                case BlockID.Bricks:
                    return true;
            }

            // Check whether instance blocks light:
            var pos = new Vector(x, y);
            foreach (var inst in level.ActiveInstances)
            {
                if (inst.BlocksLight())
                {
                    if (inst.GetTiles().Contains(pos))
                        return true;
                }
            }

            return false;
        }

        public void SetVisible(int x, int y)
        {
            if (Server.LevelManager.CurrentLevel?.IsInMapBounds(x, y) ?? false)
                VisibilityMap[x, y] = true;
        }

        public int GetDistance(int x, int y)
        {
            return new Vector(x, y).EuclidianLength();
        }
    }
}
