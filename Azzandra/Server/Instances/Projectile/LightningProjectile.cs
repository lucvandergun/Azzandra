using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class LightningProjectile : VectorTargetProjectile
    {
        protected override Symbol Symbol => new Symbol('+', Color.Aqua);
        public override string AssetName => "lightning";
        public LightningProjectile(Instance origin, Vector[] nodes) : base(origin, nodes)
        { }
    }
}
