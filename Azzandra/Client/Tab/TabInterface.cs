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
    public abstract class TabInterface
    {
        public readonly GameClient GameClient;
        protected readonly SpriteFont TitleFont = Assets.Gridfont;
        protected readonly SpriteFont Font = Assets.Medifont;

        public static Keys[] KeyInputs = new Keys[]
        {
            Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P,
            Keys.H, Keys.J, Keys.K, Keys.L, Keys.OemSemicolon,
        };

        protected virtual string Title => "Hello there!";
        protected readonly Rectangle TitleBar;
        protected readonly int TitleSectionHeight = 20;
        protected readonly Vector2 TitleOffset = new Vector2(8, 8);

        protected readonly ScrollableSurface SubArea;

        public TabInterface(GameClient gameClient)
        {
            GameClient = gameClient;
            SubArea = new ScrollableSurface(GameClient.Engine);
        }

        public virtual void OnResize(Point region)
        {
            int pad = 4;
            SubArea.SetSize(pad, TitleSectionHeight, region.X - 2 * pad, region.Y - TitleSectionHeight);
        }

        public virtual void Render(Surface surface)
        {
            bool isHoverSurface = GameClient.DisplayHandler.IsHoverSurface(surface);
            RenderSubArea(surface.Region, isHoverSurface);
            
            GameClient.Engine.GraphicsDevice.SetRenderTarget(surface.Display);
            GameClient.Engine.GraphicsDevice.Clear(Color.Black);
            GameClient.Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            // Draw title
            DrawTitle(surface);

            // Draw subarea
            Display.DrawSurface(SubArea);

            // Draw additional info
            DrawAdditional(surface);

            // Draw scrollbar
            SubArea.ScrollBar.Render(surface.Position, new Vector2(surface.Width - ScrollBar.Width / 2, TitleSectionHeight + SubArea.Height / 2), isHoverSurface);

            GameClient.Engine.SpriteBatch.End();
            GameClient.Engine.GraphicsDevice.SetRenderTarget(null);
        }

        protected void DrawTitle(Surface surface)
        {
            var title = Title;
            Display.DrawStringVCentered(TitleOffset, title, TitleFont, Color.White, true);
        }

        protected virtual void RenderSubArea(Rectangle outerRegion, bool isHoverSurface)
        {

        }

        protected virtual void DrawAdditional(Surface surface)
        {

        }
    }
}
