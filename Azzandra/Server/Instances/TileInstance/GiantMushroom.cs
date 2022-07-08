
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class GiantMushroom : Instance
    {
        public override int GetW() => 2;
        public override int GetH() => 2;

        public override Symbol GetSymbol() => new Symbol("M", Color.DarkCyan);

        public GiantMushroom(int x, int y) : base(x, y)
        {
            
        }
    }
}
