using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class IceGiant : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Ice;
        public override bool CanFlee() => false;

        public IceGiant(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('G', Color.LightBlue);
        public override string Name => "ice giant";
    }
}
