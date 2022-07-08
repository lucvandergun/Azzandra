using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class TargetProjectile : Projectile
    {
        protected readonly Instance Target;

        public TargetProjectile(Instance origin, Instance target) : base(origin)
        {
            Target = target;
        }

        public TargetProjectile(int x, int y) : base(x, y)
        {

        }
    }
}
