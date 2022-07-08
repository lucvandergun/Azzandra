using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ArmouredSkeleton : Skeleton
    {
        public override EntityType EntityType => EntityType.Undead;


        public ArmouredSkeleton(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('a', Color.LightSlateGray);
    }
}
