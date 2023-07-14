using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class MenuScene : IScene
    {
        public readonly Engine Engine;
        private Surface Surface;

        private Button[] Buttons;
        private readonly SpriteFont Font = Assets.Gridfont, TitleFont = Assets.Medifont;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 42);
        private readonly string Title = "Caverns of Azzandra";

        private MenuBackgroundRenderer Bg;

        public MenuScene(Engine engine, Point screenSize)
        {
            Engine = engine;
            Surface = new Surface(0, 0, screenSize.X, screenSize.Y, Engine);

            Buttons = new Button[]
            {
                new Button(ButtonSize, "New Game")
                {
                    OnClick = () => Engine.SetScene(new GameCreationScene(Engine, Surface.Size.ToPoint()))
                },
                new Button(ButtonSize, "Load Game")
                {
                    OnClick = () =>
                    {
                        var game = new GameClient(engine, Engine.ScreenSize, "save1.dat");
                        game.LoadGame();
                        Engine.SetScene(game);
                    },
                    CanInteract = () =>
                    {
                        if (!Directory.Exists(Engine.SAVE_DIRECTORY))
                            Directory.CreateDirectory(Engine.SAVE_DIRECTORY);
                        return File.Exists(Engine.SAVE_DIRECTORY + "save1.dat");
                    }
                },
                //new Button(ButtonSize, "Hall of Fame")
                //{
                //    OnClick = () => { },
                //    CanInteract = () => false
                //},
                new Button(ButtonSize, "Settings")
                {
                    OnClick = () => Engine.SetScene(new SettingsScene(engine, Engine.ScreenSize))
                },
                new Button(ButtonSize, "Exit")
                {
                    OnClick = () =>
                    {
                        Engine.CloseAndSaveGame();
                    }
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

            // Render Title:
            int scale = 4;
            var titlePos = new Vector2(Surface.Width / 2, Surface.Height * 2 / 11);
            titlePos -= new Vector2(Assets.CoALogo.Width, Assets.CoALogo.Height) * scale / 2;
            Display.DrawTexture(titlePos, Assets.CoALogo, scale);

            //titlePos -= TitleFont.MeasureString(Title) * scale / 2;
            //sb.DrawString(TitleFont, Title, titlePos + new Vector2(scale / 2), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            //sb.DrawString(TitleFont, Title, titlePos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Render buttons.
            var pos = new Vector2(Surface.Width / 2, Surface.Height / 2) - (Buttons.Length - 1) * ButtonOffset / 2;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].Render(Surface, pos + i * ButtonOffset, gd, sb, true);
            }


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
