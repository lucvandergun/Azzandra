using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Wolf : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;

        public override bool ReturnHome() => base.ReturnHome() && !(Parent?.Instance is AlphaWolf aw && IsInRange(aw, 3));


        public Wolf(int x, int y) : base(x, y) { }


        public override Symbol GetSymbol() => new Symbol('w', new Color(184, 184, 184));

        protected override bool IsTargetStillValid()
        {
            return Parent?.Instance is AlphaWolf aw && aw.Target?.Instance == Target?.Instance || base.IsTargetStillValid();
        }
    }
}
