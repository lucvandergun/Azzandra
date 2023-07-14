using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class FlyAnimation : IAnimation
    {
        private int FlyTimer = 0;
        private Instance _owner;
        public Instance Owner => _owner;

        public FlyAnimation(Instance owner)
        {
            _owner = owner;
        }

        public Vector2 GetDisposition()
        {
            double yOffset = Math.Sin(FlyTimer / (4 * Math.PI)) * 2 * Owner.GetH();
            return new Vector2(0, (int)yOffset);
        }

        public void Update()
        {
            FlyTimer++;
        }
    }
}
