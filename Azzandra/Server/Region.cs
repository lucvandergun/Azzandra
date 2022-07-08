using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Region
    {
        public Vector Position, Size;
        public Vector BottomRight => Position + Size;
        public int X => Position.X;
        public int Y => Position.Y;
        public int Width => Size.X;
        public int Height => Size.Y;

        public Region(Vector pos, Vector size)
        {
            Position = pos;
            Size = size;
        }

        public Region(int x, int y, int w, int h)
        {
            Position = new Vector(x, y);
            Size = new Vector(w, h);
        }

        public Microsoft.Xna.Framework.Rectangle ToRectangle()
        {
            return new Microsoft.Xna.Framework.Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        }

        public bool IsInRegion(Vector position, Vector size)
        {
            return position >= Position && position + size <= Position + Size;
        }
    }

}
