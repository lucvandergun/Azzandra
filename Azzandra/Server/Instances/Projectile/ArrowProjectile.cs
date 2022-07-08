using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ArrowProjectile : TargetProjectileMoving
    {
        protected Texture2D Sprite => Assets.Arrow;
        
        public ArrowProjectile(Instance origin, Instance target) : base(origin, target)
        {
            
        }

        public override void Draw(Vector2 pos, float lightness)
        {
            var texture = Sprite;
            Display.DrawTextureRotated(pos, texture, Color, 1, Angle);
        }
    }
}
