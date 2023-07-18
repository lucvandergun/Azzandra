using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Goblin : Enemy
    {
        public override EntityType EntityType => EntityType.Goblin;
        public override bool CanOpenDoors() => true;


        public Goblin(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('g', Color.OliveDrab);
    }
}
