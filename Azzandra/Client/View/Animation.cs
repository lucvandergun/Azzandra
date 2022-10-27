using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Animation
    {
        public Texture2D Texture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int AmtOfFrames { get; set; }
        public float Speed { get; set; }

        public Animation(Texture2D tex, float speed = 1f)
        {
            Texture = tex;
            Height = tex.Height;
            Width = Height;
            AmtOfFrames = tex.Width / Width;
            Speed = speed;
        }

        /// <summary>
        /// Draw the current frame of the animation at the desired location (centered)
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        public void Draw(SpriteBatch sb, Vector2 pos, int frame = 0, Color? colorEffect = null, float angle = 0f)
        {
            var tex = Texture ?? Assets.UnknownSprite;

            sb.Draw(
                tex,
                pos,
                new Rectangle(frame * Width, 0, Width, Height),
                colorEffect ?? Color.White,
                angle,
                new Vector2(Width, Height) / 2,
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}
