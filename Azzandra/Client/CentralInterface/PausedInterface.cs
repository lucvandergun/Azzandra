using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class PausedInterface : Interface
    {
        public override bool DisableControls => true;

        private readonly Button[] Buttons;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 42);

        public PausedInterface(GameClient gameClient) : base(gameClient)
        {
            Buttons = new Button[]
            {
                new Button(ButtonSize, "Continue")
                {
                    OnClick = () => {
                        Close();
                    }
                },
                new Button(ButtonSize, "Settings")
                {
                    OnClick = () => {
                        GameClient.DisplayHandler.Interface = new SettingsInterface(gameClient);
                    }
                },
                new Button(ButtonSize, "Controls Info")
                {
                    OnClick = () => {
                        GameClient.DisplayHandler.Interface = new ControlsInterface(gameClient);
                    }
                },
                new Button(ButtonSize, "Save and Exit")
                {
                    OnClick = () => {
                        GameClient.Engine.SetScene(new MenuScene(GameClient.Engine, GameClient.DisplayHandler.Screen.Size));
                    }
                }
            };
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.White * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

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
