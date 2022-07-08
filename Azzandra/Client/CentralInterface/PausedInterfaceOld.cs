using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class PausedInterfaceOld : Interface
    {
        public override bool DisableControls => true;

        private readonly Button[] Buttons;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 42);

        public PausedInterfaceOld(GameClient gameClient) : base(gameClient)
        {
            var dh = gameClient.DisplayHandler;
            Surface.SetSize(26 * 16, 18 * 16);
            Surface.SetPosition((dh.Screen.Width - Surface.Width) / 2, (dh.Screen.Height - Surface.Height) / 2);

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
                        
                    }
                },
                new Button(ButtonSize, "Exit to Menu")
                {
                    OnClick = () => {

                        GameClient.Exit(); // Saves game.
                        GameClient.Engine.SetScene(new MenuScene(GameClient.Engine, GameClient.DisplayHandler.Screen.Size));
                    }
                }
            };
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Draw bars
            var color = DisplayHandler.LightLineColor;
            int w = 2;
            int pad = 16 / 2 - w;
            var outerRect = new Rectangle(new Point(pad), region.Size - new Point(2 * pad));
            Display.DrawInline(outerRect, color, w);
            Display.DrawRect(16, 32 + pad, region.Width - 32, w, color);

            // Render buttons.
            var pos = new Vector2(Surface.Width / 2, Surface.Height * 0.6f) - (Buttons.Length - 1) * ButtonOffset / 2;
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
