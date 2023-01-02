using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class ButtonFormat
    {
        public abstract void DrawBackground(Rectangle rect, SpriteBatch sb);


        public class Menu : ButtonFormat
        {
            public override void DrawBackground(Rectangle rect, SpriteBatch sb)
            {
                var asset = Assets.Button;
                var assetRect = new Rectangle(0, 0, asset.Width, asset.Height);
                Vector2 assetSize = new Vector2(asset.Width, asset.Height);
                int border = 6;
                Vector2 corner = new Vector2(border + 1);
                var color = Color.White;
                
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left, rect.Top + border), new Vector2(border, rect.Height - 2 * border)), Display.MakeRectangle(new Vector2(assetRect.Left, assetRect.Top + border + 1), new Vector2(border, assetSize.Y - 2 * border - 2)), color);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left + border, rect.Top), new Vector2(rect.Width - 2 * border, border)), Display.MakeRectangle(new Vector2(assetRect.Left + border + 1, assetRect.Top), new Vector2(assetSize.X - 2 * border - 2, border)), color);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Right - border, rect.Top + border), new Vector2(border, rect.Height - 2 * border)), Display.MakeRectangle(new Vector2(assetRect.Right - border, assetRect.Top + border + 1), new Vector2(border, assetSize.Y - 2 * border - 2)), color);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left + border, rect.Bottom - border), new Vector2(rect.Width - 2 * border, border)), Display.MakeRectangle(new Vector2(assetRect.Left + border + 1, assetRect.Bottom - border), new Vector2(assetSize.X - 2 * border - 2, border)), color);

                sb.Draw(asset, new Vector2(rect.Left, rect.Top), Display.MakeRectangle(new Vector2(0, 0), corner), color);
                sb.Draw(asset, new Vector2(rect.Right - corner.X, rect.Top), Display.MakeRectangle(new Vector2(assetSize.X - corner.X, 0), corner), color);
                sb.Draw(asset, new Vector2(rect.Left, rect.Bottom - corner.Y), Display.MakeRectangle(new Vector2(0, assetSize.Y - corner.Y), corner), color);
                sb.Draw(asset, new Vector2(rect.Right - corner.X, rect.Bottom - corner.Y), Display.MakeRectangle(new Vector2(assetSize.X - corner.X, assetSize.Y - corner.Y), corner), color);

                int inner = assetRect.Width - 2 * border;
                var innerRect = new Rectangle(border, border, inner, inner);
                Vector2 innerDest = new Vector2(rect.Width - 2 * border, rect.Height - 2 * border);
                var destRect = new Rectangle(rect.Left + border, rect.Top + border, (int)innerDest.X, (int)innerDest.Y);

                for (int i, j = 0; j < innerDest.Y; j += inner)
                {
                    for (i = 0; i < innerDest.X; i += inner)
                    {
                        var loc = new Vector2(rect.Left + border + i, rect.Top + border + j);
                        var dest = Display.MakeRectangle(loc, new Vector2(Math.Min(inner, innerDest.X - i), Math.Min(inner, innerDest.Y - j)));
                        var source = Display.MakeRectangle(new Vector2(border), new Vector2(Math.Min(inner, innerDest.X - i), Math.Min(inner, innerDest.Y - j)));
                        sb.Draw(asset, dest, source, color);
                    }
                }
            }
        }

        public class Simple : ButtonFormat
        {
            public override void DrawBackground(Rectangle rect, SpriteBatch sb)
            {
                Display.DrawRect(rect, new Color(31, 31, 31));
                Display.DrawInline(rect, new Color(63, 63, 63));
            }
        }

        public class SimpleDark : ButtonFormat
        {
            public override void DrawBackground(Rectangle rect, SpriteBatch sb)
            {
                Display.DrawInline(rect, new Color(127, 127, 127), 1);
            }
        }
    }
}
