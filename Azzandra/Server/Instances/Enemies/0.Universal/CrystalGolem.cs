using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CrystalGolem : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Rock;


        public CrystalGolem(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('C', Color.DarkOrchid);
    }
}
