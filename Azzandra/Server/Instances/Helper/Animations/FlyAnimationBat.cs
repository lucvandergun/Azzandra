using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class FlyAnimationBat : IAnimation
    {
        private int FlyTimer = 0;
        private Instance _owner;
        public Instance Owner => _owner;

        public FlyAnimationBat(Instance owner)
        {
            _owner = owner;
        }

        public Vector2 GetDisposition()
        {
            double yOffset = -Math.Abs(Math.Sin(FlyTimer / (4 * Math.PI)) * 4 * Owner.GetH());
            return new Vector2(0, (int)yOffset);
        }

        public void Update()
        {
            FlyTimer++;
        }
    }
}
