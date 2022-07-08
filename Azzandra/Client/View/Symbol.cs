using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Symbol
    {
        public string Char;
        public Color Color;

        public Symbol(char c, Color col)
        {
            Char = c.ToString();
            Color = col;
        }
        public Symbol(string c, Color col)
        {
            Char = c;
            Color = col;
        }
    }
}
