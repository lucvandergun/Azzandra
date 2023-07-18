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
        public readonly GameClient GameClient;
        private readonly GraphicsDevice GraphicsDevice;
        private readonly SpriteBatch SpriteBatch;

        public const int GRID_SIZE = GameClient.GRID_SIZE;
        private readonly Vector2 TileSize = new Vector2(GRID_SIZE);
        private readonly Vector2 HalfTileSize = new Vector2(GRID_SIZE) / 2;
        private const float INVISIBILITY_LIGHTNESS = 0.2f;
        private const float CHEAT_LIGHTNESS = 0.6f;
        public Color BGColor { get; private set; } = Color.Black;
        private Temp Temp = Temp.Lukewarm;
        private void CheckUpdateBGColor()
        {
            var temp = GameClient.Server.LevelManager.CurrentLevel.Temp;
            if (temp != Temp || BGColor == Color.Black)
            {
                Temp = temp;
                BGColor = TileDisplay.GetTempColor(temp);
            }
        }

        public Vector? HoverPos { get; private set; } // In-game coordinates.
        public Instance HoverInstance { get; private set; }
        public void SetHoverInstance(Instance instance) => HoverInstance = instance;

        public readonly Color[] MarkerColors = new Color[] { Color.Green, Color.GreenYellow, Color.Yellow, Color.Orange, Color.Red };


        public TileDisplayManager[,] TileDisplays { get; private set; }
        private int TileDisplayWidth, TileDisplayHeight;
        public TileDisplayManager GetTileDisplay(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < TileDisplayWidth && y < TileDisplayHeight)
                return TileDisplays[x, y];
            return null;
        }


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
            var level = server.LevelManager.CurrentLevel;

            // Update tile animations:
            UpdateTileDisplays();

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

            // Start drawing
            GraphicsDevice.SetRenderTarget(surface.Display);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var viewOffset = -GetRealPlayerPos(server) + surface.Size / 2;
            
            // Calculate mouse hover position
            HoverPos = CalculateHoverPos(Input.MousePosition - surface.Position, viewOffset, isHoverSurface);
            HoverInstance = null;

            // Draw tilemap:
            DrawTileDisplays(x, y, w, h, viewOffset);

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
            var visibleInstances = GameClient.IsLighted
                ? server.LevelManager.CurrentLevel.ActiveInstances.CreateCopy()
                : server.User.VisibilityHandler.VisibleInstances.CreateCopy();
            bool clickThrough = Input.IsMouseRightDown || Input.IsKeyDown[Keys.LeftShift];

            visibleInstances.Sort((a, b) => (a is Entity ? 1 : 0) - (b is Entity ? 1 : 0));
            foreach (var inst in visibleInstances)
            {
                if (GameClient.IsDebug)
                    DrawSightSquares(inst, viewOffset);
                float lightness = INVISIBILITY_LIGHTNESS.LerpTo(1f, inst.GetTiles().Max(t => level.GetTileLightness(t)) / LightLevelCalculator.MAX_STRENGTH);
                if (GameClient.IsLighted)
                    lightness = Math.Max(CHEAT_LIGHTNESS, lightness);
                inst.DrawView(SpriteBatch, viewOffset, server, lightness);

                // Calculate hover & target instance
                if (isHoverSurface && GameClient.DisplayHandler.MouseInterface == null && targetingMode is TargetingMode.InstanceTargeting it &&  (inst != server.User.Player || it.CanTargetPlayer))
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


        private void UpdateTileDisplays()
        {
            var level = GameClient.Server.LevelManager.CurrentLevel;
            var vh = GameClient.Server.User.VisibilityHandler;
            int w = level.MapWidth, h = level.MapHeight;

            // Create new TileDisplay Matrix if needed (fill it with memory tiles)
            if (TileDisplays == null || TileDisplayWidth != w || TileDisplayHeight != h)
            {
                TileDisplays = new TileDisplayManager[w, h];
                TileDisplayWidth = w;
                TileDisplayHeight = h;
                for (int i, j = 0; j < h; j++)
                {
                    for (i = 0; i < w; i++)
                    {
                        TileDisplays[i, j] = new TileDisplayManager(vh.GetMemoryTile(i, j));
                    }
                }
            }


            // Update each of the tiles
            TileDisplayManager td;
            Tile corresponding;
            for (int i, j = 0; j < h; j++)
            {
                for (i = 0; i < w; i++)
                {
                    td = TileDisplays[i, j];

                    // Update visible tiles only:
                    if (GameClient.IsLighted || vh.IsTileVisible(i, j))
                    {
                        // Update the tile display if outdated:
                        corresponding = GameClient.IsLighted ? level.GetTile(i, j) : level.GetMemoryTile(i, j);
                        if (td == null || td.Tile != corresponding)
                        {
                            TileDisplays[i, j] = new TileDisplayManager(corresponding);
                        }

                        if (td.AnimationManager.AmtOfLoops == 0 && td.AnimationManager.Animation?.AmtOfFrames > 0)
                            td.Play();

                        td.Update();
                    }
                    else
                    {
                        td.Stop();
                    }
                }
            }
        }

        public void OnNewFloor()
        {
            // Check whether to update the background color based on the current floor's temp:
            CheckUpdateBGColor();

            // Make sure the previous tile displays are all disposed of
            TileDisplays = null;
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


        private void DrawTileDisplays(int x, int y, int w, int h, Vector2 viewOffset)
        {
            Vector2 pos;
            float lightness;

            var vh = GameClient.Server.User.VisibilityHandler;
            var level = GameClient.Server.LevelManager.CurrentLevel;

            for (int i, j = y; j < y + h; j++)
            {
                for (i = x; i < x + w; i++)
                {
                    // Get tile & draw to screen
                    pos = new Vector2(i, j) * GRID_SIZE + viewOffset + HalfTileSize;

                    var tileD = GetTileDisplay(i, j);
                    if (tileD != null)
                    {
                        lightness = GetDrawnTileLightness(vh, level, i, j);
                        tileD.Draw(SpriteBatch, this, pos, Color.White, lightness);
                    }
                }
            }
        }

        public float GetDrawnTileLightness(VisibilityHandler vh, Level level, int x, int y)
        {
            if (GameClient.IsLighted)
            {
                return vh.IsTileVisible(x, y) ? 1f : CHEAT_LIGHTNESS;
            }
            else
            {
                return vh.IsTileVisible(x, y)
                ? INVISIBILITY_LIGHTNESS.LerpTo(1f, level.GetTileLightness(new Vector(x, y)) / LightLevelCalculator.MAX_STRENGTH)
                : INVISIBILITY_LIGHTNESS;
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
                return (mousePos - viewOffset).ToInt() / GRID_SIZE;
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
            Display.DrawStringCentered(pos, symbol.Char, Assets.Gridfont, symbol.Color.ChangeBrightness(-1 + lightness)); //symbol.Color.BlendWith(Color.Black, 1 - lightness)
        }

        private void DrawAsset(Vector2 pos, Texture2D asset, float lightness)
        {
            Display.DrawSprite(pos, asset, Color.White.ChangeBrightness(-1 + lightness)); //symbol.Color.BlendWith(Color.Black, 1 - lightness)
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
