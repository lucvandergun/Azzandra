using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class VectorTargetProjectile : Projectile
    {
        protected virtual Symbol Symbol => new Symbol('+', Color.Aqua);
        protected readonly Vector[] Nodes;
        public override bool IsOnPlayerTick => Origin?.IsOnPlayerTick ?? false;
        public override bool RenderLightness => false;


        public VectorTargetProjectile(Instance origin, Vector[] nodes) : base(origin)
        {
            X = nodes.First().X;
            Y = nodes.First().Y;
            Nodes = nodes;
        }
        
        public override void TickEnd()
        {
            Destroy();
        }


        // Rendering:
        public override void Draw(Vector2 pos, float lightness = 1)
        {
            //Vector2 dist = (End - Start).ToFloat() * ViewHandler.GRID_SIZE;
            //int length = (End - Start).ChebyshevLength();
            //for (int i = 0; i <= length; i++)
            //    Display.DrawInstanceString(pos + dist / length * i, Symbol.Char, Assets.Gridfont, Symbol.Color, 1, 0f, true);

            foreach (var node in Nodes)
            {
                Display.DrawInstanceString((node - Position).ToFloat() * ViewHandler.GRID_SIZE + pos, Symbol.Char, Assets.Gridfont, Symbol.Color, 1, 0f, true);
            }
        }
    }
}
