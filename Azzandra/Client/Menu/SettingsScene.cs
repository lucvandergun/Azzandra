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
    public class SettingsScene : IScene
    {
        public readonly Engine Engine;
        private Surface Surface;

        private SettingsRenderer SettingsRenderer;

        private MenuBackgroundRenderer Bg;

        public SettingsScene(Engine engine, Point screenSize)
        {
            Engine = engine;
            Surface = new Surface(0, 0, screenSize.X, screenSize.Y, Engine);

            SettingsRenderer = new SettingsRenderer(Engine, Engine.Settings);
            SettingsRenderer.SetBackButtonEffect(() => Engine.SetScene(new MenuScene(engine, Engine.ScreenSize)));

            Bg = new MenuBackgroundRenderer(engine, new Rectangle(Point.Zero, screenSize));
        }

        public void Update(GameTime gameTime)
        {
            Bg.Update();
        }

        public void OnResize(Point screenSize)
        {
            Surface.SetSize(screenSize.X, screenSize.Y);
            Bg.OnResize(new Rectangle(Point.Zero, screenSize));
        }

        public RenderTarget2D Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb)
        {
            gd.SetRenderTarget(Surface.Display);
            gd.Clear(Color.Black);
            Bg.Render(gd, sb);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            SettingsRenderer.Render(Surface, gd, sb);

            sb.End();
            gd.SetRenderTarget(null);

            return Surface.Display;
        }

        public void Exit()
        {
            Engine.SaveClientSettings();
        }
    }
}
