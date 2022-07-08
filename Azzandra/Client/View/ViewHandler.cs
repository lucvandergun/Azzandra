using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public enum Corner { UpLeft, UpRight, DownLeft, DownRight }

    public class ViewHandler
    {
        private readonly GameClient GameClient;
        private readonly GraphicsDevice GraphicsDevice;
        private readonly SpriteBatch SpriteBatch;

        public const int GRID_SIZE = GameClient.GRID_SIZE;
        private readonly Vector2 TileSize = new Vector2(GRID_SIZE);
        private readonly Vector2 HalfTileSize = new Vector2(GRID_SIZE) / 2;
        private const float INVISIBILITY_LIGHTNESS = 0.2f;
        private const float CHEAT_LIGHTNESS = 0.6f;

        public Vector? HoverPos { get; private set; } // In-game coordinates.
        public Instance HoverInstance { get; private set; }
        public void SetHoverInstance(Instance instance) => HoverInstance = instance;

        private Color[] MarkerColors = new Color[] { Color.Green, Color.GreenYellow, Color.Yellow, Color.Orange, Color.Red };


        public ViewHandler(GameClient gameClient)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;
        }

        public void Update()
        {
            UpdateColorStates();
        }


        public void Render(Surface surface)
        {
            var server = GameClient.Server;
            if (server == null)
                return;
            
            bool isHoverSurface = GameClient.DisplayHandler.IsHoverSurface(surface);
            var targetingMode = GameClient.InputHandler.TargetingMode;

            // Render view texture:

            // Calculate size; ceil division + two tiles bigger than screen
            int w = surface.Width / GRID_SIZE + 3;
            int h = surface.Height / GRID_SIZE + 3;

            // Get player coordinates as view center
            int x, y;
            var player = server.User.Player;
            if (player != null)
            {
                x = player.X - w / 2;
                y = player.Y - h / 2;
            }
            else
            {
                x = 0;
                y = 0;
            }

            //Start drawing
            GraphicsDevice.SetRenderTarget(surface.Display);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var viewOffset = -GetRealPlayerPos(server) + surface.Size / 2;
            
            //Calculate mouse hover position
            HoverPos = CalculateHoverPos(Input.MousePosition, viewOffset, isHoverSurface);
            HoverInstance = null;

            // Draw tile map:
            if (GameClient.IsLighted)
                DrawRealTileMap(x, y, w, h, viewOffset);
            else
                DrawMemoryTileMap(x, y, w, h, viewOffset);


            // Tile target handling:
            if (targetingMode is TargetingMode.TileTargeting tt)
            {
                // Mouse handling
                if (HoverPos != null)
                {
                    Display.DrawTexture((HoverPos.Value.ToFloat() * GRID_SIZE) + viewOffset, Assets.GetTargetSprite(GRID_SIZE), Color.White);
                    if (Input.IsMouseLeftPressed)
                    {
                        tt.TileTarget = HoverPos.Value - player.Position;

                        tt.PerformTargetAction(GameClient.InputHandler);
                    }
                }

                // Draw
                if (tt.TileTarget != null)
                {
                    var absPos = player.Position + tt.TileTarget;
                    var realPos = (absPos.Value.ToFloat() * GRID_SIZE) + viewOffset;
                    Display.DrawTexture(realPos, Assets.GetTargetSprite(GRID_SIZE), Color.Yellow);
                }
            }


            // Render instances
            var level = server.LevelManager.CurrentLevel;
            var visibleInstances = GameClient.IsLighted
                ? server.LevelManager.CurrentLevel.ActiveInstances.CreateCopy()
                : server.User.VisibilityHandler.VisibleInstances.CreateCopy();
            bool clickThrough = Input.IsMouseRightDown || Input.IsKeyDown[Keys.LeftShift];

            visibleInstances.Sort((a, b) => (a is Entity ? 1 : 0) - (b is Entity ? 1 : 0));
            foreach (var inst in visibleInstances)
            {
                //if (Engine.IsDebug) DrawSightSquares(inst, viewOffset);
                float lightness = INVISIBILITY_LIGHTNESS.LerpTo(1f, inst.GetTiles().Max(t => level.GetTileLightness(t)) / LightLevelCalculator.MAX_STRENGTH);
                if (GameClient.IsLighted)
                    lightness = Math.Max(CHEAT_LIGHTNESS, lightness);
                inst.DrawView(viewOffset, server, lightness);

                // Calculate hover & target instance
                if (isHoverSurface && GameClient.DisplayHandler.MouseItem == null && targetingMode is TargetingMode.InstanceTargeting it &&  (inst != server.User.Player || it.CanTargetPlayer))
                {
                    var clickBox = GetClickBox(inst);
                    if (clickBox != null)
                    {
                        if (Input.MouseHover(surface.Position + (inst.CalculateRealPos(server) + viewOffset) - clickBox.Value / 2, clickBox.Value))
                        {
                            HoverInstance = inst;

                            if (Input.IsMouseLeftPressed && (inst is Entity && !clickThrough || !(inst is Entity)))
                            {
                                // If clicked on target, or targetingmode has an inbound action:
                                if (server.User.Target == inst || it.InboundAction != null)
                                    it.PerformAction(GameClient.InputHandler, inst);
                                    //server.SetPlayerAction(new ActionInstance(player, inst, GameClient.InputHandler.IsShift));
                                else
                                    server.User.SetTarget(inst);
                            }
                        }
                    }
                }
            }

            if (targetingMode is TargetingMode.InstanceTargeting)
            {
                // Draw hover & target instance clickboxes:
                if (HoverInstance != null)
                {
                    DrawClickBox(HoverInstance.CalculateRealPos(server) + viewOffset, HoverInstance, Color.White);
                }
                if (server.User.Target != null)
                {
                    if (server.User.Player.CanSee(server.User.Target))
                        DrawClickBox(server.User.Target.CalculateRealPos(server) + viewOffset, server.User.Target, Color.Yellow);
                }
            }


            // Draw hitsplats:
            foreach (Entity c in server.LevelManager.CurrentLevel.ActiveInstances.Where(i => i is Entity))
            {
                for (int i = c.Hits.Count - 1; i >= 0; i--)
                    c.Hits[i].Update();

                if (visibleInstances.Contains(c))
                    for (int i = c.Hits.Count - 1; i >= 0; i--)
                        c.Hits[i].Draw(viewOffset + c.CalculateRealPos(server), server);

                //var hitPos = viewOffset;
                //foreach (var hit in c.Hits)
                //{
                //    hit.Draw(hitPos + c.CalculateRealPos(server), server);
                //    hitPos.Y -= 12;
                //}
            }


            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawSightSquares(Instance inst, Vector2 viewOffset)
        {
            if (inst is Entity c)
            {
                if (c.SightSquares != null)
                {
                    foreach (var p in c.SightSquares)
                    {
                        var pos = new Vector2(p.X, p.Y) * GRID_SIZE + viewOffset;
                        DrawTileTexture(pos + HalfTileSize, Assets.TileBasic, Color.LightYellow * 0.5F);
                    }
                }
            }
        }

        /// <summary>
        /// Gets instance clickbox size based on instance type.
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public Vector2? GetClickBox(Instance inst)
        {
            if (inst == null)
                return null;
            
            //if (inst == Engine.User.Player)
            //    return null;

            if (inst.CanBeTargetedByPlayer())
            {
                if (inst.GetW() >= 3)
                    return new Vector2(48);

                if (inst.GetW() == 2)
                    return new Vector2(32);

                return new Vector2(16);
            }

            return null;
        }

        /// <summary>
        /// Draws a clickbox in requested color at specified location.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="inst"></param>
        /// <param name="color"></param>
        public void DrawClickBox(Vector2 pos, Instance inst, Color color)
        {
            var clickBox = GetClickBox(inst);

            if (clickBox == null)
                return;

            var sprite =
                  clickBox.Equals(new Vector2(24)) ? Assets.Target24
                : clickBox.Equals(new Vector2(32)) ? Assets.Target32
                : clickBox.Equals(new Vector2(48)) ? Assets.Target48
                : Assets.Target16;

            Display.DrawTexture(pos - clickBox.Value / 2, sprite, color);
        }


        private void DrawMemoryTileMap(int x, int y, int w, int h, Vector2 viewOffset)
        {
            Tile tile;
            Vector2 pos;
            float lightness;

            var vh = GameClient.Server.User.VisibilityHandler;
            var level = GameClient.Server.LevelManager.CurrentLevel;

            for (int i, j = y; j < y + h; j++)
            {
                for (i = x; i < x + w; i++)
                {
                    // Get tile & draw to screen
                    pos = new Vector2(i, j) * GRID_SIZE + viewOffset;
                    tile = GameClient.Server.User.VisibilityHandler.GetMemoryTile(i, j);
                    //lightness = Engine.Server.User.VisibilityHandler.IsTileVisible(i, j) ? 1f : INVISIBILITY_LIGHTNESS;

                    lightness = vh.IsTileVisible(i, j)
                        ? INVISIBILITY_LIGHTNESS.LerpTo(1f, level.GetTileLightness(new Vector(i, j)) / LightLevelCalculator.MAX_STRENGTH)
                        : INVISIBILITY_LIGHTNESS;

                    DrawTile(pos, tile, lightness);
                }
            }
        }

        private void DrawRealTileMap(int x, int y, int w, int h, Vector2 viewOffset)
        {
            Tile tile;
            Vector2 pos;
            float lightness;

            var vh = GameClient.Server.User.VisibilityHandler;
            var level = GameClient.Server.LevelManager.CurrentLevel;

            for (int i, j = y; j < y + h; j++)
            {
                for (i = x; i < x + w; i++)
                {
                    //Get tile & draw to screen
                    pos = new Vector2(i, j) * GRID_SIZE + viewOffset;
                    tile = level.GetTile(i, j);
                    lightness = vh.IsTileVisible(i, j) ? 1f : CHEAT_LIGHTNESS;

                    DrawTile(pos, tile, lightness);
                }
            }
        }


        private Vector GetPlayerPos(Server server)
        {
            if (server != null)
            {
                var player = server.User.Player;
                if (player != null)
                    return new Vector(player.X, player.Y);
            }
            return Vector.Zero;
        }
        private Vector2 GetRealPlayerPos(Server server)
        {
            if (server != null)
            {
                // Absolute player position - used to calculate view offset
                var player = server.User.Player;
                if (player != null)
                    return player.CalculateRealPos(server);
            }
            
            return Vector2.Zero;
        }


        private Vector? CalculateHoverPos(Vector2 mousePos, Vector2 viewOffset, bool isHover)
        {
            if (!isHover)
                return null;
            else
                return (mousePos - viewOffset + new Vector2(GRID_SIZE / 4, GRID_SIZE)).ToInt() / GRID_SIZE - new Vector(1, 3);
        }

        private void DrawTile(Vector2 pos, Tile tile, float lightness)
        {
            pos += HalfTileSize;

            //Get tile display attributes
            var floorDisplay = TileDisplay.Get(tile.Ground.ID);

            //Draw floor tile background
            DrawBackground(pos, floorDisplay.BGColor * lightness);


            //Draw tile marker if Debugging:
            if (GameClient.IsDebug && tile.Marker > 0)
            {
                int i = Math.Max(0, Math.Min(tile.Marker - 1, MarkerColors.Length - 1));
                DrawTileTexture(pos, Assets.TileBasic, MarkerColors[i] * 0.5f);
            }

            //Draw tile symbol (prioritize object over floor symbol
            if (tile.Object.ID != BlockID.Void)
            {
                var objectDisplay = TileDisplay.Get(tile.Object.ID);
                if (objectDisplay.Symbol != null)
                {
                    DrawSymbol(pos, objectDisplay.Symbol, lightness);
                    return;
                }
            }

            if (floorDisplay.Symbol != null)
                DrawSymbol(pos, floorDisplay.Symbol, lightness);
        }

        private void DrawBackground(Vector2 pos, Color col)
        {
            DrawTileTexture(pos, Assets.TileBasic, col);
        }

        private void DrawTileTexture(Vector2 pos, Texture2D tex, Color col, int deg = 0)
        {
            if (tex == null)
                return;

            var tileRad = new Vector2(16 / 2);
            float scale = GRID_SIZE / 16f;
            SpriteBatch.Draw(tex, pos, tex.Bounds, col, MathHelper.ToRadians(deg), tileRad, scale, SpriteEffects.None, 0f);
        }


        private void DrawSymbol(Vector2 pos, Symbol symbol, float lightness)
        {
            Display.DrawStringCentered(pos, symbol.Char, Assets.Gridfont, symbol.Color * lightness);
        }


        private int ColorState30 = 0;
        private int ColorState60 = 0;
        public static Color FireColor = Color.Orange;
        public static Color LavaColor = Color.Orange;
        public static Color GetFireColor => FireColor;
        public static Color GetLavaColor => LavaColor;

        private void UpdateColorStates()
        {
            if (ColorState30 < 30)
                ColorState30++;
            else
                ColorState30 = 0;

            if (ColorState60 < 240)
                ColorState60++;
            else
                ColorState60 = 0;

            float value30 = (float)Math.Abs(ColorState30 - 15) / 15f;
            float value60 = (float)Math.Abs(ColorState60 - 120) / 120f;

            FireColor = Color.DarkOrange.BlendWith(Color.OrangeRed, value30);
            LavaColor = Color.OrangeRed.BlendWith(Color.DarkOrange, value60) * 0.6f;
        }

    }
}
