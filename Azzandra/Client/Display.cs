using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Azzandra
{
    public enum TileRotation { Up, Left, Down, Right }

    public static class Display
    {
        private static readonly  SpriteBatch SpriteBatch = Program.Engine.SpriteBatch;

        public static void DrawTexture(int x, int y, Texture2D tex, float scale = 1, float rotation = 0) { DrawTexture(x, y, tex, Color.White, scale, rotation); }
        public static void DrawTexture(Vector2 pos, Texture2D tex, float scale = 1, float rotation = 0) { DrawTexture(pos, tex, Color.White, scale, rotation); }
        public static void DrawTexture(int x, int y, Texture2D tex, Color color, float scale = 1, float rotation = 0) { DrawTexture(new Vector2(x, y), tex, color, scale, rotation); }
        public static void DrawTexture(Vector2 pos, Texture2D tex, Color color, float scale = 1, float rotation = 0)
        {
            if (tex != null)
            {
                SpriteBatch.Draw(tex, pos, null, color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }

        public static void DrawTextureRotated(Vector2 pos, Texture2D tex, Color color, float scale = 1, float rotation = 0)
        {
            if (tex != null)
            {
                SpriteBatch.Draw(tex, pos, null, color, rotation, new Vector2(tex.Width, tex.Height) / 2, scale, SpriteEffects.None, 0);
            }
        }


        public static void DrawString(int x, int y, string str, SpriteFont font, bool shadow = false)
        {
            DrawString(new Vector2(x, y), str, font, Color.White, shadow);
        }
        public static void DrawString(int x, int y, string str, SpriteFont font, Color col, bool shadow = false)
        {
            DrawString(new Vector2(x, y), str, font, col, shadow);
        }
        public static void DrawString(Vector2 pos, string str, SpriteFont font, bool shadow = false)
        {
            DrawString(pos, str, font, Color.White, shadow);
        }
        public static void DrawString(Vector2 pos, string str, SpriteFont font, Color col, bool shadow = false)
        {
            if (font != null && !string.IsNullOrEmpty(str))
            {
                if (shadow) SpriteBatch.DrawString(font, str, pos + Vector2.One, Color.Black);
                SpriteBatch.DrawString(font, str, pos, col);
            }
        }

        public static void DrawStringCentered(Vector2 pos, string str, SpriteFont font, bool shadow = false)
        {
            DrawStringCentered(pos, str, font, Color.White, shadow);
        }
        public static void DrawStringCentered(Vector2 pos, string str, SpriteFont font, Color col, bool shadow = false)
        {
            if (font != null && !string.IsNullOrEmpty(str))
            {
                pos -= new Vector2(Util.GetStringWidth(str, font), Util.GetStringHeight(str, font)) / 2;

                if (shadow) SpriteBatch.DrawString(font, str, pos + Vector2.One, Color.Black);
                SpriteBatch.DrawString(font, str, pos, col);
            }
        }
        public static void DrawInstanceString(Vector2 pos, string str, SpriteFont font, Color col, float scale, float rotation = 0f, bool shadow = false)
        {
            if (font != null && !string.IsNullOrEmpty(str))
            {
                var scaleVec = scale == 1f ? new Vector2(scale) : new Vector2(scale, scale * 0.75f); // Don't know why y-axis *= 0.75f
                pos -= font.MeasureString(str) * scaleVec / 2;

                if (shadow) SpriteBatch.DrawString(font, str, pos + Vector2.One, Color.Black, rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                SpriteBatch.DrawString(font, str, pos, col, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        public static void DrawStringVCentered(Vector2 pos, string str, SpriteFont font, bool shadow = false)
        {
            DrawStringVCentered(pos, str, font, Color.White, shadow);
        }
        public static void DrawStringVCentered(Vector2 pos, string str, SpriteFont font, Color col, bool shadow = false)
        {
            if (font != null && !string.IsNullOrEmpty(str))
            {
                pos -= new Vector2(0, Util.GetFontHeight(font) / 2);

                if (shadow) SpriteBatch.DrawString(font, str, pos + Vector2.One, Color.Black);
                SpriteBatch.DrawString(font, str, pos, col);
            }
        }

        public static void DrawStringVCenteredRight(Vector2 pos, string str, SpriteFont font, bool shadow = false)
        {
            DrawStringVCenteredRight(pos, str, font, Color.White, shadow);
        }
        public static void DrawStringVCenteredRight(Vector2 pos, string str, SpriteFont font, Color col, bool shadow = false)
        {
            if (font != null && !string.IsNullOrEmpty(str))
            {
                pos -= new Vector2(Util.GetStringWidth(str, font), Util.GetFontHeight(font) / 2);

                if (shadow) SpriteBatch.DrawString(font, str, pos + Vector2.One, Color.Black);
                SpriteBatch.DrawString(font, str, pos, col);
            }
        }

        public static Rectangle MakeRectangle(Vector2 pos, Vector2 size) => new Rectangle(pos.ToPoint(), size.ToPoint());
        public static void DrawRect(int x, int y, int w, int h, Color col) { DrawRect(new Rectangle(x, y, w, h), col); }
        public static void DrawRect(Vector2 start, Vector2 size, Color col) { DrawRect(new Rectangle(start.ToPoint(), size.ToPoint()), col); }
        public static void DrawRect(Rectangle r, Color col)
        {
            SpriteBatch.Draw(Assets.Rectangle, r, col);
        }

        public static void DrawHorizontalLine(Vector2 start, int length, Color col)
        {
            DrawRect(new Rectangle((int)start.X, (int)start.Y, length, 1), col);
        }
        public static void DrawVerticalLine(Vector2 start, int height, Color col)
        {
            DrawRect(new Rectangle((int)start.X, (int)start.Y, 1, height), col);
        }


        public static void DrawSurface(Surface surface)
        {
            SpriteBatch.Draw(surface.Display, surface.Region, Color.White);
        }


        public static void DrawOutline(Rectangle area, Color col, int w = 1)
        {
            //draws a line of width around specified rectangle

            //left and right line draw the corners
            DrawRect(new Rectangle(area.Left - w, area.Top - w, w, area.Height + w * 2), col);
            DrawRect(new Rectangle(area.Right, area.Top - w, w, area.Height + w * 2), col);
            DrawRect(new Rectangle(area.Left, area.Top - w, area.Width, w), col);
            DrawRect(new Rectangle(area.Left, area.Bottom, area.Width, w), col);
        }

        public static void DrawInline(Rectangle area, Color col, int w = 1)
        {
            //draws a line of width inside specified rectangle

            DrawRect(new Rectangle(area.Left, area.Top, w, area.Height), col);
            DrawRect(new Rectangle(area.Right - w, area.Top, w, area.Height), col);
            DrawRect(new Rectangle(area.Left, area.Top, area.Width, w), col);
            DrawRect(new Rectangle(area.Left, area.Bottom - w, area.Width, w), col);
        }




        //public static bool DrawButton(Surface surface, Vector2 pos, Vector2 size, string str, Texture2D asset, bool isActivated, bool canHover, bool canActivate = true)
        //{
        //    //this method draws a button with its topleft at provided pos.

        //    var rect = new Rectangle(pos.ToPoint(), size.ToPoint());

        //    bool hover = Input.MouseHover(surface.Position + pos, size) && canHover;
        //    var color = canActivate && hover ? Color.Aqua : Color.White;

        //    //drawing stuff
        //    Display.DrawRect(rect, new Color(31, 31, 31));
        //    Display.DrawInline(rect, new Color(63, 63, 63));


        //    //hover & selected overlay
        //    if (canActivate && hover && Input.IsMouseLeftDown)
        //        Display.DrawRect(rect, Color.White * 0.35f);
        //    else if (isActivated)
        //        Display.DrawRect(rect, Color.White * 0.25f);

        //    //draw asset & string
        //    var drawPos = pos + size / 2;
        //    if (asset != null)
        //    {
        //        Display.DrawTexture(drawPos - new Vector2(16, 24), asset, 2);
        //        drawPos.Y = pos.Y + size.Y - 8;
        //    }
        //    Display.DrawStringCentered(drawPos, str, Assets.Gridfont, color, true);

        //    //not activatable overlay
        //    if (!canActivate)
        //        Display.DrawRect(rect, Color.Black * 0.5f);

        //    //return whether clicked
        //    bool clicked = canActivate && hover && Input.IsMouseLeftPressed;

        //    return clicked;
        //}




        //public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        //public static void DrawString(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color)
        //{
        //    Vector2 size = font.MeasureString(text);
        //    Vector2 pos = bounds.Center.ToVector2();
        //    Vector2 origin = size * 0.5f;

        //    if (align.HasFlag(Alignment.Left))
        //        origin.X += bounds.Width / 2 - size.X / 2;

        //    if (align.HasFlag(Alignment.Right))
        //        origin.X -= bounds.Width / 2 - size.X / 2;

        //    if (align.HasFlag(Alignment.Top))
        //        origin.Y += bounds.Height / 2 - size.Y / 2;

        //    if (align.HasFlag(Alignment.Bottom))
        //        origin.Y -= bounds.Height / 2 - size.Y / 2;

        //    SpriteBatch.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        //}
    }
}
