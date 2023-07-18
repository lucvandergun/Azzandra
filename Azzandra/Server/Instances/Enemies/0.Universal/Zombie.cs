using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Zombie : Enemy
    {
        public override EntityType EntityType => EntityType.Undead;
        public override bool CanFlee() => false;
        public override bool CanOpenDoors() => true;

        public Zombie(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('z', Color.CornflowerBlue);
    }
}
