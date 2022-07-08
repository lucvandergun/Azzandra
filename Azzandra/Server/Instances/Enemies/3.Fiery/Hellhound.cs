using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Hellhound : Enemy
    {
        public override EntityType EntityType => EntityType.Demon;


        public Hellhound(int x, int y) : base(x, y)
        { }


        public override Symbol GetSymbol() => new Symbol('H', Color.Crimson);
    }
}
