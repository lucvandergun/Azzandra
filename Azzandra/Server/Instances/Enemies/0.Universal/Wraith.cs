using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Wraith : Enemy
    {
        public override EntityType EntityType => EntityType.Spirit;


        public Wraith(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol("W", Color.White);
    }
}
