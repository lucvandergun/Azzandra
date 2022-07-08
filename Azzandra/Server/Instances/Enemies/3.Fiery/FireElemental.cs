using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class FireElemental : Enemy
    {
        public override EntityType EntityType => EntityType.Fire;
        public override MoveType GetMovementType() => MoveType.Fly;

        public FireElemental(int x, int y) : base(x, y)
        { }


        public override Symbol GetSymbol() => new Symbol('e', Color.Orange);
    }
}
