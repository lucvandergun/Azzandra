using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Azzandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Azzandra
{
    public class Minimap
    {
        private readonly GameClient GameClient;
        public Surface Surface;
        private Color[,] Colors;
        private int MapWidth, MapHeight;
        private const int SCALE = 2;
        
        public Minimap(GameClient gameClient)
        {
            GameClient = gameClient;
            Surface = new Surface(gameClient.Engine);
        }

        public void Update()
        {
            var level = GameClient.Server?.LevelManager.CurrentLevel;
            if (level == null) return;

            // Setup surface:
            Surface.SetSize(MapWidth * SCALE, MapHeight * SCALE);

            // Add memory tilemap:
            var map = GameClient.IsLighted ? level.TileMap : level.MemoryTileMap;
            MapWidth = map.GetLength(0);
            MapHeight = map.GetLength(1);
            Colors = new Color[MapWidth, MapHeight];

            for (int i, j = 0; j < MapHeight; j++)
            {
                for (i = 0; i < MapWidth; i++)
                {
                    Colors[i, j] = GetColor(map[i, j]) ?? Color.Black;
                }
            }

            // Add selected instance markers:
            foreach (var door in level.ActiveInstances.Where(i => i is Door && level.IsInMapBounds(i) && map[i.X, i.Y].Ground.ID != BlockID.Void))
                Colors[door.X, door.Y] = new Color(128, 64, 0);
            if (GameClient.Server?.User.Player != null)
            {
                int px = GameClient.Server.User.Player.X, py = GameClient.Server.User.Player.Y;
                if (level.IsInMapBounds(px, py))
                    Colors[px, py] = Color.Lime;
            }
            Colors[level.StartPosition.X, level.StartPosition.Y] = Color.White;
            if (map[level.EndPosition.X, level.EndPosition.Y].Ground.ID != BlockID.Void)
                Colors[level.EndPosition.X, level.EndPosition.Y] = Color.Aqua;
        }

        private Color? GetColor(Tile tile)
        {
            if (tile.Object.ID == BlockID.Dock)
                return new Color(76, 38, 0);
            switch (tile.Object.ID)
            {
                default:
                    break;
                case BlockID.Dock:
                    return new Color(76, 38, 0);
                case BlockID.Pillar:
                    return new Color(127, 127, 127);
            }
            switch (tile.Ground.ID)
            {
                default:
                    return Color.Black;
                case BlockID.Wall:
                    return new Color(72, 69, 72);
                case BlockID.Pillar: case BlockID.Bricks:
                    return new Color(127, 127, 127);
                case BlockID.Water:
                    return TileDisplay.Get(BlockID.Water).BGColor;
                case BlockID.Ice:
                    return TileDisplay.Get(BlockID.Ice).BGColor;
                case BlockID.Lava:
                    return TileDisplay.Get(BlockID.Lava).BGColor;
                case BlockID.Sulphur:
                    return TileDisplay.Get(BlockID.Sulphur).BGColor;
                case BlockID.Poison:
                    return TileDisplay.Get(BlockID.Poison).BGColor;
            }
        }

        public void Render(int x, int y, GraphicsDevice gd, SpriteBatch sb)
        {
            Surface.SetPosition(x, y);
            gd.SetRenderTarget(Surface.Display);
            gd.Clear(Color.White * 0f);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            for (int i, j = 0; j < MapHeight; j++)
            {
                for (i = 0; i < MapWidth; i++)
                {
                    Display.DrawRect(i * SCALE, j * SCALE, SCALE, SCALE, Colors[i, j]); 
                }
            }

            sb.End();
            gd.SetRenderTarget(null);
        }
    }
}
