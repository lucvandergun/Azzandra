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
        public void Draw(SpriteBatch sb, Vector2 pos, int frame = 0, float scale = 1f, Color? colorEffect = null, float angle = 0f)
        {
            var tex = Texture ?? Assets.UnknownSprite;

            sb.Draw(
                tex,
                pos,
                new Rectangle(frame * Width, 0, Width, Height),
                colorEffect ?? Color.White,
                angle,
                new Vector2(Width, Height) / 2,
                scale,
                SpriteEffects.None,
                0f);
        }


        /// <summary>
        /// Draw a 16x16 icon of this instance at the specified location.
        /// Always draw the first frame of an animation, and will extract a subset for sprites larger than 16x16.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        public void DrawIcon(SpriteBatch sb, Vector2 pos)
        {
            var tex = Texture ?? Assets.UnknownSprite;

            int x = (Width - 16) / 2;
            int y = 0;
            var rect = new Rectangle(x, y, Math.Min(Width, 16), Math.Min(Height, 16));

            sb.Draw(
                tex,
                pos,
                rect,
                Color.White,
                0f,
                new Vector2(8),
                1f,
                SpriteEffects.None,
                0f);
        }
    }
}
