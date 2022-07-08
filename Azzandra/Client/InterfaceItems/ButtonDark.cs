using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ButtonDark : Button
    {
        public ButtonDark(Vector2 size, string defaultText) : base(size, defaultText)
        {
            
        }

        //protected override void DrawForm(Rectangle rect)
        //{
        //    //Display.DrawRect(rect, new Color(31, 31, 31));
        //    Display.DrawInline(rect, new Color(127, 127, 127), 1);
        //}
    }
}
