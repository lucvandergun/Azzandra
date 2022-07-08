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
    public class MenuScene2 : IScene
    {
        public readonly Engine Engine;
        private Surface Surface;

        private Button NewButton, LoadButton;
        private Vector2 buttonSize = new Vector2(96, 48);

        private MenuBackgroundRenderer Bg;

        public MenuScene2(Engine engine, Point screenSize)
        {
            Engine = engine;
            Surface = new Surface(0, 0, screenSize.X, screenSize.Y, Engine);

            NewButton = new Button(buttonSize, "New\nGame")
            {
                OnClick = () =>
                {
                    Engine.SetScene(new GameCreationScene(Engine, Surface.Size.ToPoint()));
                }
            };

            LoadButton = new Button(buttonSize, "Load\nGame")
            {
                OnClick = () =>
                {
                    var game = new GameClient(engine, Engine.ScreenSize, "save1.dat");
                    game.LoadGame();
                    Engine.SetScene(game);
                }
            };

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

            NewButton.Render(Surface, new Vector2(Surface.Width / 2 - 64, Surface.Height / 2), gd, sb, true);
            LoadButton.Render(Surface, new Vector2(Surface.Width / 2 + 64, Surface.Height / 2), gd, sb, true);

            sb.End();
            gd.SetRenderTarget(null);

            return Surface.Display;
        }

        public void Exit()
        {
            // Nothing happens here.
        }
    }
}
