using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class ButtonFormat
    {
        public readonly static Action<Rectangle> Menu = (rect) =>
        {
            Display.DrawRect(rect, new Color(31, 31, 31));
            Display.DrawInline(rect, new Color(63, 63, 63));
        };
        public readonly static Action<Rectangle> Dark = (rect) => Display.DrawInline(rect, new Color(127, 127, 127), 1);

    }
}
