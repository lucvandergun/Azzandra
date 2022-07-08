using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GameOverInterface : Interface
    {
        public override bool DisableControls => true;
        public override bool CanBeClosed => false;

        private readonly Button[] Buttons;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 42);

        private readonly string Title = "You have died!";

        public GameOverInterface(GameClient gameClient) : base(gameClient)
        {
            Buttons = new Button[]
            {
                new Button(ButtonSize, "Exit to Menu")
                {
                    OnClick = () => {
                        GameClient.Engine.SetScene(new MenuScene(GameClient.Engine, GameClient.DisplayHandler.Screen.Size));
                    }
                }
            };
        }

        public override void Update()
        {
            base.Update();

            if (GameClient.IsDevMode && Input.IsKeyPressed[Keys.C])
            {
                Close();
                GameClient.Server?.User.Respawn(GameClient.Server?.LevelManager.CurrentLevel);
            }
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.White * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Render Title:
            int scale = 4;
            var titlePos = new Vector2(Surface.Width / 2, Surface.Height * 2 / 11) - Font.MeasureString(Title) * scale / 2;
            sb.DrawString(Font, Title, titlePos + new Vector2(scale / 2), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            sb.DrawString(Font, Title, titlePos, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Render buttons.
            var pos = new Vector2(Surface.Width / 2, Surface.Height / 2) - (Buttons.Length - 1) * ButtonOffset / 2;
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].Render(Surface, pos + i * ButtonOffset, gd, sb, true);
            }
            

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public override void Destroy()
        {
            
        }
    }
}
