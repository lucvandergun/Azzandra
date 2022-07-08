using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Dir
    {
        private int _x, _y;
        public int X { get => _x; set => _x = Math.Sign(value); }
        public int Y { get => _y; set => _y = Math.Sign(value); }

        public Dir(int x, int y) { _x = Math.Sign(x); _y = Math.Sign(y); }

        public bool IsNull()
        {
            return X == 0 && Y == 0;
        }

        public bool IsDiagonal()
        {
            return X != 0 && Y != 0;
        }

        public static Dir operator + (Dir a, Dir b)
        {
            return new Dir(a.X + b.X, a.Y + b.Y);
        }
        public static Dir operator - (Dir a, Dir b)
        {
            return new Dir(a.X - b.X, a.Y - b.Y);
        }



        public Vector2 ToFloat()
        {
            return new Vector2(X, Y);
        }
        public Vector ToVector()
        {
            return new Vector(X, Y);
        }
        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }

        /// <summary>
        /// Get a random direction (one of four orthogonals).
        /// </summary>
        public static Dir Random => new Dir(Util.Random.Next(-1, 2), Util.Random.Next(-1, 2));
    }
}
