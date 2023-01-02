using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class MenuBackgroundRenderer
    {
        public readonly Engine Engine;

        // Backround rendering:
        private Rectangle ScreenBounds;
        private Texture2D Background;
        private Vector2 BgPos; // The draw position relative to the top-left of the background image.
        private Vector2 BgDir;
        private int BgTime = 0, BgOffTime = 0;
        private const int BG_RENDER_TIME = 15 * 60;
        private const int BG_TRANSITION_TIME = 60; // This is part of BG_RENDER_TIME, handled at the start and at the end of it.
        private const int BG_OFF_TIME = 1 * 30;
        private const float BG_SPEED = 0.75f;

        public MenuBackgroundRenderer(Engine engine, Rectangle screenBounds)
        {
            Engine = engine;
            ScreenBounds = screenBounds;
            BgDir = GetRandomBgDir();
        }

        public void Update()
        {
            // Update Bg time
            if (BgOffTime == 0)
            {
                BgTime = BG_RENDER_TIME;
                BgOffTime = -1;

                Background = Assets.GetRandomMenuBackground(Background);
                BgTime = BG_RENDER_TIME;
                BgPos = GetRandomBgPos(ScreenBounds, Background);
                BgDir = GetRandomBgDir();
            }
            else if (BgTime == 0)
            {
                BgOffTime = BG_OFF_TIME;
                BgTime = -1;
            }

            if (BgTime > 0) BgTime--;
            if (BgOffTime > 0) BgOffTime--;

            // Update Bg pos
            if (IsPosOutsideBounds(ScreenBounds, Background, BgPos))
            {
                // Pick new bg pos if it's invalid
                BgPos = GetRandomBgPos(ScreenBounds, Background);
                BgDir = GetRandomBgDir();
            }
            {
                BgDir = GetNextBgDir(ScreenBounds, Background, BgPos, BgDir);
                BgPos += BgDir * BG_SPEED;
            }
        }

        private Vector2 GetRandomBgPos(Rectangle bounds, Texture2D bg)
        {
            return new Vector2(
                Util.Random.Next(0, Math.Max(0, bounds.Width - bg.Width)),
                Util.Random.Next(0, Math.Max(0, bounds.Height - bg.Height)));
        }

        private Vector2 GetRandomBgDir()
        {
            var a = Util.Random.NextDouble() * 2 * Math.PI;
            return new Vector2(
                (float)Math.Sin(a),
                (float)Math.Cos(a));
        }

        private Vector2 GetNextBgDir(Rectangle bounds, Texture2D bg, Vector2 pos, Vector2 dir)
        {
            var newPos = pos + dir * BG_SPEED;
            if (IsOutsideHorizontalBound(bounds, bg, newPos))
                return new Vector2(-dir.X, dir.Y);
            else if (IsOutsideVerticalBound(bounds, bg, newPos))
                return new Vector2(dir.X, -dir.Y);
            else
                return dir;
        }

        private float GetBgLightness(int time)
        {
            return (time < BG_TRANSITION_TIME) ? ((float)time / (float)BG_TRANSITION_TIME)
                : (time > BG_RENDER_TIME - BG_TRANSITION_TIME) ? (float)(BG_RENDER_TIME - time) / (float)BG_TRANSITION_TIME
                : 1f;
        }

        private bool IsOutsideHorizontalBound(Rectangle bounds, Texture2D bg, Vector2 pos) => pos.X < -bg.Width / 4 || pos.X >= bounds.Width - bg.Width / 4 * 3;
        private bool IsOutsideVerticalBound(Rectangle bounds, Texture2D bg, Vector2 pos) => pos.Y < -bg.Height / 4 || pos.Y >= bounds.Height - bg.Height / 4 * 3;

        private bool IsPosOutsideBounds(Rectangle bounds, Texture2D bg, Vector2 pos)
        {
            return IsOutsideHorizontalBound(bounds, bg, pos) || IsOutsideVerticalBound(bounds, bg, pos);
        }


        public void OnResize(Rectangle screenBounds)
        {
            ScreenBounds = screenBounds;
        }

        public void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, null);
            Display.DrawTexture(BgPos, Background, Color.White * GetBgLightness(BgTime));

            // Draw 'infinite' arches:
            Texture2D arch = Assets.MenuArch;
            Texture2D archBg = Assets.MenuArchBg;
            var drawOffset = new Vector2(0, -32);
            int xStart = ((ScreenBounds.Width - arch.Width) / 2);
            while (xStart > 0) xStart -= arch.Width;
            for (int i, j = 0; j < ScreenBounds.Height + 32; j += arch.Height)
            {
                for (i = xStart; i < ScreenBounds.Width; i += arch.Width)
                {
                    var pos = new Vector2(i, j);
                    Display.DrawTexture(pos + drawOffset, archBg);
                    Display.DrawTexture(pos + drawOffset, arch, new Color(127, 127, 127));
                }
            }

            sb.End();
        }
    }
}
