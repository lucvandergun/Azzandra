using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ToxicSpider : Enemy
    {
        public override EntityType EntityType => EntityType.Spider;

        public ToxicSpider(int x, int y) : base(x, y) { }

        public override Symbol GetSymbol() => new Symbol('t', Color.GreenYellow);
    }
}
