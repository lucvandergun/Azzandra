using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct TextFormat
    {
        public Color DefaultColor;
        public SpriteFont Font;
        public Alignment Alignment;
        public bool IsShadow;

        public TextFormat(Color defaultColor, SpriteFont font, Alignment alignment, bool isShadow = false)
        {
            DefaultColor = defaultColor;
            Font = font;
            Alignment = alignment;
            IsShadow = isShadow;
        }
    }
}
