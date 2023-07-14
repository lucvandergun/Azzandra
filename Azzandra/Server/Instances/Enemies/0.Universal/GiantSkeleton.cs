using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GiantSkeletonWarrior : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Skeleton;
        public override bool CanFlee() => false;
        public override int AggressiveRange => 5;


        public GiantSkeletonWarrior(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('S', Color.White);
    }

    public class GiantSkeletonArcher : GiantSkeletonWarrior
    {
        public GiantSkeletonArcher(int x, int y) : base(x, y) { }
    }

    public class GiantSkeletonWarlock : GiantSkeletonWarrior
    {
        public GiantSkeletonWarlock(int x, int y) : base(x, y) { }
    }
}
