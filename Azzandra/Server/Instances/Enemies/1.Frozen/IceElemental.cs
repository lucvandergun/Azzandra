using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class IceElemental : Enemy
    {
        public override EntityType EntityType => EntityType.Ice;
        public override MoveType StartingMoveType => MoveType.Fly;
        public override bool CanFlee() => false;

        public IceElemental(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('e', Color.LightBlue);
        public override string Name => "ice elemental";
    }
}
