using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Fiend : Kobold
    {
        public override EntityType EntityType => EntityType.Demon;
        public override MoveType StartingMoveType => MoveType.Fly;

        public Fiend(int x, int y) : base(x, y) { }
    }
}
