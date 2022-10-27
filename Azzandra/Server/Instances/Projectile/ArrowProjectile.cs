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
        public ArrowProjectile(Instance origin, Instance target) : base(origin, target, Color.White, "arrow")
        {
            
        }

        //public override void Draw(SpriteBatch sb, Vector2 pos, float lightness)
        //{
        //    var texture = Sprite;
        //    Display.DrawSprite(pos, texture, Color, 1, Angle);
        //}
    }
}
