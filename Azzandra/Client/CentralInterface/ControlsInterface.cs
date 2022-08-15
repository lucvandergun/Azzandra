using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ControlsInterface : Interface
    {
        public override bool DisableControls => true;

        private readonly Button BackButton;
        private Vector2 ButtonSize = new Vector2(192, 32);
        private Vector2 ButtonOffset = new Vector2(0, 42);
        private Tuple<string, string>[] Commands;
        private Vector2 CommandOffset = new Vector2(0, 24);

        public ControlsInterface(GameClient gameClient, string text = "Close", Action action = null) : base(gameClient)
        {
            BackButton = new Button(ButtonSize, text)
            {
                OnClick = () => { Close(); action?.Invoke(); }
            };
            Commands = new Tuple<string, string>[]
            {
                Tuple.Create("Pause game", "<aqua>Esc<r> - To save the game or access the settings."),
                Tuple.Create("Controls/help", "<aqua>?<r> - Directly opens the controls page."),
                Tuple.Create("Movement", "<aqua>AWSD<r>, <aqua>Z<r> or <aqua>Numpad<r> - Can also bump to attack or interact."),
                Tuple.Create("", " <slate>(Add <aqua>Shift<slate> or <aqua>Ctrl<slate> for non-solids next to you, or leap attacks.)"),
                Tuple.Create("Resting", "<aqua>R<r> - Starts a rest action for 10 turns (gets interrupted when attacked)."),
                Tuple.Create("Quick-swap", "<aqua>Q<r> - Makes you swap to your previously held weapons."),
                Tuple.Create("Targeting", "<aqua>T<r> - Targets the next nearest instance around you."),
                Tuple.Create("Target Action", "<aqua>Space<r> - Attacks or interacts with the current target instance."),
                Tuple.Create("", " <slate>(Add <aqua>Shift<slate> or <aqua>Ctrl<slate> for leap attacks.)"),
            };
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.White * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Render buttons.
            //var pos = new Vector2(Surface.Width / 2, Surface.Height / 2) - (Buttons.Length - 1) * ButtonOffset / 2;
            //for (int i = 0; i < Buttons.Length; i++)
            //{
            //    Buttons[i].Render(Surface, pos + i * ButtonOffset, gd, sb, true);
            //}
            var pos = new Vector2(Surface.Width / 2 - 200, Surface.Height / 2) - (Commands.Length - 1) * CommandOffset / 2;
            var text = new TextDrawer(pos, 24, new TextFormat(Color.LightGray, Assets.Medifont, Alignment.VCentered, true));
            foreach (var line in Commands)
            {
                text.Draw("<white>" + line.Item1);
                text.MoveX(100);
                text.DrawLine(line.Item2);
                text.SetPosition(new Vector2(pos.X, text.Pos.Y));
            }

            BackButton.Render(Surface, new Vector2(Surface.Width / 2, Surface.Height / 2) + (Commands.Length - 1) * CommandOffset / 2 + ButtonOffset, gd, sb, true);


            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public override void Destroy()
        {
            
        }
    }
}
