using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ShadowElemental : Enemy
    {
        public override EntityType EntityType => EntityType.Shadow;
        public override MoveType StartingMoveType => MoveType.Fly;
        public override int AggressiveRange => 5;
        public override bool CanFlee() => false;
        public ShadowElemental(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('s', Color.SlateGray);
    }
}
