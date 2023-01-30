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
        public abstract void DrawBackground(SpriteBatch sb, Rectangle rect, bool isSelected, bool isClicked, bool canInteract);


        public class Menu : ButtonFormat
        {
            public override void DrawBackground(SpriteBatch sb, Rectangle rect, bool isSelected, bool isClicked, bool canInteract)
            {
                Color centerColor = isClicked ? new Color(191, 191, 191) : new Color(61, 61, 61);
                Color borderColor = isSelected ? Color.White : centerColor;

                if (!canInteract)
                {
                    centerColor = centerColor.ChangeBrightness(-1f) * 0.5f;
                    borderColor = centerColor;
                }

                DrawBackground(sb, rect, centerColor, borderColor);
            }

            public void DrawBackground(SpriteBatch sb, Rectangle rect, Color centerColor, Color borderColor)
            {
                var asset = Assets.Button;
                var assetRect = new Rectangle(0, 0, asset.Width, asset.Height);
                Vector2 assetSize = new Vector2(asset.Width, asset.Height);
                int border = 6;
                Vector2 corner = new Vector2(border + 1);

                // Sides:
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left, rect.Top + border), new Vector2(border, rect.Height - 2 * border)), Display.MakeRectangle(new Vector2(assetRect.Left, assetRect.Top + border + 1), new Vector2(border, assetSize.Y - 2 * border - 2)), borderColor);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left + border, rect.Top), new Vector2(rect.Width - 2 * border, border)), Display.MakeRectangle(new Vector2(assetRect.Left + border + 1, assetRect.Top), new Vector2(assetSize.X - 2 * border - 2, border)), borderColor);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Right - border, rect.Top + border), new Vector2(border, rect.Height - 2 * border)), Display.MakeRectangle(new Vector2(assetRect.Right - border, assetRect.Top + border + 1), new Vector2(border, assetSize.Y - 2 * border - 2)), borderColor);
                sb.Draw(asset, Display.MakeRectangle(new Vector2(rect.Left + border, rect.Bottom - border), new Vector2(rect.Width - 2 * border, border)), Display.MakeRectangle(new Vector2(assetRect.Left + border + 1, assetRect.Bottom - border), new Vector2(assetSize.X - 2 * border - 2, border)), borderColor);

                // Corners
                sb.Draw(asset, new Vector2(rect.Left, rect.Top), Display.MakeRectangle(new Vector2(0, 0), corner), borderColor);
                sb.Draw(asset, new Vector2(rect.Right - corner.X, rect.Top), Display.MakeRectangle(new Vector2(assetSize.X - corner.X, 0), corner), borderColor);
                sb.Draw(asset, new Vector2(rect.Left, rect.Bottom - corner.Y), Display.MakeRectangle(new Vector2(0, assetSize.Y - corner.Y), corner), borderColor);
                sb.Draw(asset, new Vector2(rect.Right - corner.X, rect.Bottom - corner.Y), Display.MakeRectangle(new Vector2(assetSize.X - corner.X, assetSize.Y - corner.Y), corner), borderColor);

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
                        sb.Draw(asset, dest, source, centerColor);
                    }
                }
            }
        }

        public class Simple : ButtonFormat
        {
            public override void DrawBackground(SpriteBatch sb, Rectangle rect, bool isSelected, bool isClicked, bool canInteract)
            {
                Display.DrawRect(rect, new Color(31, 31, 31));
                Display.DrawInline(rect, new Color(63, 63, 63));
            }
        }

        public class SimpleDark : ButtonFormat
        {
            public override void DrawBackground(SpriteBatch sb, Rectangle rect, bool isSelected, bool isClicked, bool canInteract)
            {
                Display.DrawInline(rect, new Color(127, 127, 127), 1);
            }
        }
    }
}
