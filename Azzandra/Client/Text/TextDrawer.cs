using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public enum Alignment { TopLeft, VCentered, Centered, VCenteredRight }

    public class TextDrawer
    {
        /// <summary>
        /// The position the current line is originating from. Moving to the next line will reset the current position to this, with Y += line height.
        /// </summary>
        public Vector2 Pos { get; set; }

        /// <summary>
        /// The current actual real-time positon to draw on.
        /// </summary>
        public Vector2 CurrentPos { get; set; }

        public TextFormat Format;
        public int LineH;
        public bool ResetColorOnCall { get; set; } = false;
        public Color LastColor { set; get; } = Color.White; // Remembers the last color when new calls should not reset the color to the default color.

        // Getters and setters
        public SpriteFont Font { set => Format.Font = value; get => Format.Font; }
        public Color DefaultColor { set => Format.DefaultColor = value; get => Format.DefaultColor; }
        //public Color DefaultColor { set => Format.DefaultColor = value; get => Format.DefaultColor; }


        // Constructors:

        public TextDrawer(Vector2 pos, int lineH, Alignment alignment, SpriteFont font, Color defaultColor, bool shadow = false)
        {
            var format = new TextFormat(defaultColor, font, alignment, shadow);
            Setup(pos, lineH, format);
        }
        public TextDrawer(int xStart, int yStart, int lineH, Alignment alignment, SpriteFont font, Color defaultColor, bool shadow = false)
        {
            var pos = new Vector2(xStart, yStart);
            var format = new TextFormat(defaultColor, font, alignment, shadow);
            Setup(pos, lineH, format);
        }
        public TextDrawer(Vector2 pos, int lineH, TextFormat format)
        {
            Setup(pos, lineH, format);
        }

        public void Setup(Vector2 pos, int lineH, TextFormat format)
        {
            Pos = pos;
            LineH = lineH;
            Format = format;

            LastColor = DefaultColor;
            CurrentPos = pos;
        }


        public void SetPosition(int x, int y)
        {
            Pos = new Vector2(x, y);
            CurrentPos = Pos;
        }
        public void SetPosition(Vector2 pos)
        {
            Pos = pos;
            CurrentPos = Pos;
        }

        public void MoveX(int amt)
        {
            Pos = new Vector2(Pos.X + amt, Pos.Y);
            CurrentPos = Pos;
        }
        public void MoveY(int amt)
        {
            Pos = new Vector2(Pos.X, Pos.Y + amt);
            CurrentPos = Pos;
        }

        /// <summary>
        /// Moves the current position down by the line height.
        /// </summary>
        public void MoveLine()
        {
            MoveY(LineH);
        }

        /// <summary>
        /// Moves the current position down by the line height / 2.
        /// </summary>
        public void Skip()
        {
            MoveY(LineH / 2);
        }


        public void Draw(string str, Color? blendColor = null, float blendAmount = 1.0f)
        {
            Color? startColor = null;   // Has to happen this way due to C# version.
            if (!ResetColorOnCall) startColor = LastColor;

            var t = TextFormatter.DrawString(CurrentPos, str, Format, startColor, blendColor, blendAmount);
            CurrentPos = t.Item1;
            if (!ResetColorOnCall)
                LastColor = t.Item2;
        }


        public void DrawLine(string str, Color? blendColor = null, float blendAmount = 1.0f)
        {
            Draw(str, blendColor, blendAmount);

            CurrentPos = Pos;
            MoveLine();
        }

        public void DrawHorizontalBar(int width, int height, Color color)
        {
            var rect = new Rectangle((int)Pos.X, (int)Pos.Y - LineH / 4, width, height);
            Display.DrawRect(rect, color);
            Skip();
        }
    }
}
