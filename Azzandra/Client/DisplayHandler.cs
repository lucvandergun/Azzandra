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
    public class DisplayHandler
    {
        public enum Tab { Inventory, Equipment };

        public readonly GameClient GameClient;
        private readonly GraphicsDevice GraphicsDevice;
        private readonly SpriteBatch SpriteBatch;


        public Rectangle Screen;
        public Surface MainSurface, HeaderSurface, ViewSurface, LogSurface, BottomBarSurface, InfoSurface, MenuSurface, InputSurface, DebugSurface;

        public static Color LineColor = new Color(31, 31, 31);
        public static Color FillColor = new Color(31, 31, 31);

        public static Color DarkLineColor = LineColor; // new Color(15, 68, 21);
        public static Color LightLineColor = new Color(127, 127, 127); // new Color(43, 104, 69);



        public ViewHandler ViewHandler { get; private set; }
        public InfoRenderer InfoRenderer { get; private set; }
        public TabHandler TabHandler { get; private set; }
        public EnvironmentInterface EnvironmentInterface { get; private set; }
        public DebugRenderer DebugRenderer { get; private set; }
        public Minimap Minimap { get; private set; }

        // Trackers
        public Surface HoverSurface { get; private set; }
        private List<Surface> CurrentSurfaces = new List<Surface>();

        public Tab CurrentTab;
        public Interface Interface { get; set; }
        public ChatInterface ChatInterface { get; set; }
        public IMouseInterface MouseInterface { get; set; }
        

        public DisplayHandler(GameClient gameClient, Point screenSize)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;

            // Create sub-handlers
            ViewHandler = new ViewHandler(GameClient);
            InfoRenderer = new InfoRenderer(GameClient);
            TabHandler = new TabHandler(GameClient);
            EnvironmentInterface = new EnvironmentInterface(GameClient);
            DebugRenderer = new DebugRenderer(GameClient);
            Minimap = new Minimap(GameClient);

            InitializeSurfaces(screenSize);
        }

        private void SetSurfaceBounds(Point screenSize)
        {
            var bounds = new Rectangle(Point.Zero, screenSize);
            int size = 16;
            int pad = 16;

            // The ratios:
            int horizontalDiv = 22 * size; // From the right, left side of middle column.
            int verticalDiv = Math.Max(bounds.Height / 5 / size * size, 8 * size);  // From the bottom
            int infoHeight = 6 * size;     
            int minInvHeight = 14 * size;
            int inputHeight = GameClient.Engine.Settings.DisplayInput
                ? 6 * size
                : Math.Min(bounds.Height / 3 / size * size, bounds.Height - 4 * pad - minInvHeight - infoHeight);

            int logPadding = 8;

            var header = new Rectangle(pad, pad, bounds.Width - 2 * pad, 1 * size);
            var logArea = new Rectangle(pad, bounds.Height - verticalDiv, bounds.Width - horizontalDiv - pad, verticalDiv - pad);
            var log = new Rectangle(logArea.X + logPadding, logArea.Y + logPadding, logArea.Width - 2 * logPadding, logArea.Height - 2 * logPadding);
            var info = new Rectangle(bounds.Right - horizontalDiv + pad, header.Bottom + pad, horizontalDiv - 2 * pad, infoHeight);
            var input = new Rectangle(bounds.Right - horizontalDiv + pad, bounds.Height - (pad + inputHeight), horizontalDiv - 2 * pad, inputHeight);
            var menu = new Rectangle(bounds.Right - horizontalDiv + pad, info.Bottom + pad, horizontalDiv - 2 * pad, input.Top - info.Bottom - 2 * pad);

            var viewArea = new Rectangle(pad, header.Bottom + pad, bounds.Right - horizontalDiv - pad, logArea.Top - (header.Bottom + pad));
            //Point viewSize = new Point(37 * size, 23 * size);
            //Point viewOffset = ((viewArea.Size - viewSize).ToVector2() / 2).ToPoint();
            var view = viewArea; // new Rectangle(viewArea.Location + viewOffset, viewSize);

            // Setting:
            Screen = bounds;
            MainSurface.SetBounds(bounds);
            DebugSurface.SetBounds(viewArea);
            HeaderSurface.SetBounds(header);
            ViewSurface.SetBounds(view);
            LogSurface.SetBounds(log);
            InfoSurface.SetBounds(info);
            MenuSurface.SetBounds(menu);
            InputSurface.SetBounds(input);

            TabHandler.OnResize(MenuSurface.Size.ToPoint());
            EnvironmentInterface.OnResize(
                    GameClient.Engine.Settings.DisplayInput
                    ? menu.Size
                    : InputSurface.Size.ToPoint()
                );
        }

        private void InitializeSurfaces(Point screenSize)
        {
            MainSurface = new Surface(GameClient.Engine);
            HeaderSurface = new Surface(GameClient.Engine);
            ViewSurface = new Surface(GameClient.Engine);
            LogSurface = new Surface(GameClient.Engine);
            InfoSurface = new Surface(GameClient.Engine);
            MenuSurface = new Surface(GameClient.Engine);
            InputSurface = new Surface(GameClient.Engine);
            DebugSurface = new Surface(GameClient.Engine);
            DebugSurface.CanHover = false;

            SetSurfaceBounds(screenSize);
        }

        public void OnResize(Point screenSize)
        {
            SetSurfaceBounds(screenSize);

            // Update interfaces!
            Interface?.OnResize(screenSize);
            ChatInterface?.OnResize(screenSize);
        }

        public void Update()
        {
            ViewHandler.Update();
            Minimap.Update();

            if (Interface != null) Interface.Update();
            if (ChatInterface != null) ChatInterface.Update();

            // Compile list of current surfaces
            CurrentSurfaces = new List<Surface>()
            {
                HeaderSurface,
                ViewSurface,
                LogSurface,
                InfoSurface,
                MenuSurface,
                InputSurface
            };

            //if (ChatInterface != null) CurrentSurfaces.Add(ChatInterface.Surface);
            //if (Interface != null) CurrentSurfaces.Add(Interface.Surface);
            //if (MouseItem != null) CurrentSurfaces.Add(MouseItem.Surface);

            DetermineHoverSurface();
        }

        public void RenderDisplay(GameTime gameTime)
        {
            bool DarkenScreen = Interface?.DisableControls ?? false;
            RenderSurfaces(gameTime);
            
            GraphicsDevice.SetRenderTarget(MainSurface.Display);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Draw view border
            //int viewPad = 16;
            //Display.DrawOutline(ViewSurface.Region, FillColor, viewPad);
            //Display.DrawOutline(ViewSurface.Region, LineColor);
            //var rect = new Rectangle(ViewSurface.X - viewPad, ViewSurface.Y - viewPad, ViewSurface.Width + 2 * viewPad, ViewSurface.Height + 2 * viewPad);
            //Display.DrawInline(rect, LineColor);

            // Draw interface borders:
            int w = 2;
            int pad = 16 / 2 - w;
            var outerRect = new Rectangle(new Point(pad), Screen.Size - new Point(2 * pad));
            Display.DrawInline(outerRect, DarkLineColor, w);

            // Middle bar:
            Display.DrawRect(InfoSurface.Region.Left - (pad + w), HeaderSurface.Region.Bottom + 16, 2, Screen.Height - 3 * 16 - HeaderSurface.Height, DarkLineColor);

            // Horizontal bars:
            foreach (var surface in new List<Surface>() { HeaderSurface, InfoSurface, MenuSurface} )
            {
                Display.DrawRect(surface.Region.Left, surface.Region.Bottom + pad, surface.Region.Width, w, DarkLineColor);
            }

            // Draw all surfaces
            foreach (var surface in CurrentSurfaces)
            {
                Display.DrawSurface(surface);
            }

            // Draw minimap
            Display.DrawSurface(Minimap.Surface);

            // Draw log bars
            int length = LogSurface.Width + 2;
            var color = new Color(191, 191, 191);
            Display.DrawHorizontalLine(LogSurface.Position + new Vector2(-1, -1), length, color);
            Display.DrawHorizontalLine(LogSurface.Position + new Vector2(-1, LogSurface.Height + 1), length, color);


            // Draw debug info
            if (GameClient.IsDebug)
                Display.DrawSurface(DebugSurface);

            // Draw Cheat Mode
            if (GameClient.IsCheatMode)
                Display.DrawStringVCenteredRight(ViewSurface.Position + new Vector2(ViewSurface.Width - 4, 8), "- Cheat Mode -", Assets.Medifont, Color.Red, true);

            // Draw Interface surface
            if (DarkenScreen)
                Display.DrawRect(Screen, Color.Black * 0.9f);
            if (Interface != null)
                Display.DrawSurface(Interface.Surface);

            // Draw Chat surface
            if (ChatInterface != null)
                Display.DrawSurface(ChatInterface.Surface);

            // Darken surfaces for item menu
            if (MouseInterface != null)
            {
                if (MouseInterface is ItemMenu)
                {
                    Display.DrawRect(MenuSurface.Region, Color.Black * 0.75f);
                    Display.DrawRect(InputSurface.Region, Color.Black * 0.75f);
                }

                var surface = MouseInterface.GetSurface();
                Display.DrawSurface(surface);
                if (surface.Outline)
                    Display.DrawInline(surface.Region, LightLineColor);
            }

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private void RenderSurfaces(GameTime gameTime)
        {
            // Render all surfaces
            ViewHandler.Render(ViewSurface);
            RenderLog(LogSurface);
            InfoRenderer.Render(InfoSurface);
            TabHandler.RenderTab(MenuSurface);
            RenderHeader(HeaderSurface);

            Minimap.Render(
                LogSurface.Region.Right - Minimap.Surface.Region.Width - 16,
                LogSurface.Y + (LogSurface.Region.Height - Minimap.Surface.Region.Height) / 2,
                GraphicsDevice,
                SpriteBatch);

            if (GameClient.Engine.Settings.DisplayInput)
                GameClient.InputHandler.Render(InputSurface, GraphicsDevice, SpriteBatch);
            else
                EnvironmentInterface.Render(InputSurface);

            //if (InfoScreen != null) InfoScreen.Render(GraphicsDevice, SpriteBatch);
            if (ChatInterface != null) ChatInterface.Render();
            if (Interface != null) Interface.Render(GraphicsDevice, SpriteBatch);
            if (MouseInterface != null) MouseInterface.Render();

            if (GameClient.IsDebug)
                DebugRenderer.Render(DebugSurface, gameTime);
        }


        private void DetermineHoverSurface()
        {
            Surface hover = null;

            if (Interface?.DisableControls ?? false)
            {
                HoverSurface = Interface.Surface;
                return;
            }

            var surfaces = CurrentSurfaces.CreateCopy();
            if (Interface != null) surfaces.Add(Interface.Surface);
            if (ChatInterface != null) surfaces.Add(ChatInterface.Surface);
            if (MouseInterface != null) surfaces.Add(MouseInterface.GetSurface());

            foreach (var surface in surfaces)
                if (surface.CanHover && Input.MouseHover(surface.Region))
                    hover = surface;

            HoverSurface = hover;
        }

        public bool IsHoverSurface(Surface surface)
        {
            return surface == HoverSurface;
        }

        private void RenderLog(Surface surface)
        {
            surface.SetAsRenderTarget();
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            GameClient.Log.Render(surface);

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }


        private void RenderHeader(Surface surface)
        {
            surface.SetAsRenderTarget();
            surface.Clear();
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Draw dungeon info
            int padding = 8;
            var pos = new Vector2(padding, surface.Height / 2);
            var text = new TextDrawer(pos, 16, new TextFormat(Color.White, Assets.Gridfont, Alignment.VCentered));

            text.Draw("<slate>Caverns of Azzandra<r>");

            if (GameClient.Server != null)
            {
                var depth = GameClient.Server?.LevelManager.Depth ?? 0;
                var temp = GameClient.Server.LevelManager.LevelTemperatures[depth - 1].ToString();
                text.Draw(" - Depth: " + depth + ", Atmosphere: " + temp);
            }
            else
                text.Draw(" - No world loaded");


            // Draw menu tabs:
            TabHandler.RenderMenu(HeaderSurface);

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public void CloseInterface()
        {
            Interface.Close();
        }
    }
}
