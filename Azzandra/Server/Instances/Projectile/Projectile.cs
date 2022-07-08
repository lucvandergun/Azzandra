using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Projectile : Instance
    {
        protected readonly Instance Origin;
        public override bool IsOnPlayerTick => Origin?.IsOnPlayerTick ?? false;

        public Projectile(Instance origin) : base(origin.X, origin.Y)
        {
            Origin = origin;
        }

        public Projectile(int x, int y) : base(x, y)
        {

        }

        // Rendering:
    }
}
