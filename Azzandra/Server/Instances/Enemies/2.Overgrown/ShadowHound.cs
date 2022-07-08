using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ShadowHound : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;


        public ShadowHound(int x, int y) : base(x, y)
        { }


        public override Symbol GetSymbol() => new Symbol('s', Color.DarkSlateGray);
    }
}
