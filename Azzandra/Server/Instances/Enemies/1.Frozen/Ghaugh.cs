using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Ghaugh : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;


        public Ghaugh(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('g', Color.LightSteelBlue); //Azure
    }
}
