using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class VampireBat : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;
        public override MoveType StartingMoveType => MoveType.Fly;


        public VampireBat(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('v', Color.Maroon); //RosyBrown
    }
}
