using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Azzandra
{
    public class Surface
    {
        public static Engine Engine;
        public static GraphicsDevice GraphicsDevice;
        public static SpriteBatch SpriteBatch;

        public Vector2 Position;
        public RenderTarget2D Display { get; protected set; }
        public Rectangle Region => new Rectangle((int)Position.X, (int)Position.Y, Display.Width, Display.Height);
        public Rectangle SelfRegion => new Rectangle(0, 0, Display.Width, Display.Height);
        public int X => (int)Position.X;
        public int Y => (int)Position.Y;
        public int Width => Display.Width;
        public int Height => Display.Height;
        public Vector2 Size => new Vector2(Width, Height);

        public bool CanHover { get; set; } = true;  // Whether to take this surface into account when calculating user mouse hover
        public bool Outline { get; set; } = false;  // Whether to auto draw an outline


        public Surface(int x, int y, int w, int h, Engine engine)
        {
            Init(engine);
            SetSize(x, y, w, h);
        }

        public Surface(Rectangle rect, Engine engine) { Init(engine); SetBounds(rect);}
        public Surface(Engine engine) { Init(engine); SetSize(0, 0, 1, 1); }

        private void Init(Engine engine)
        {
            Engine = engine;
            GraphicsDevice = Engine.GraphicsDevice;
            SpriteBatch = Engine.SpriteBatch;
        }


        public void SetBounds(Rectangle rect)
        {
            SetPosition(rect.X, rect.Y);
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, rect.Width), Math.Max(1, rect.Height));
        }

        public void SetSize(int x, int y, int w, int h)
        {
            SetPosition(x, y);
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, w), Math.Max(1, h));
        }

        public void SetSize(Vector2 pos, Vector2 size)
        {
            SetPosition((int)pos.X, (int)pos.Y);
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, (int)size.X), Math.Max(1, (int)size.Y));
        }

        public void SetSize(Rectangle rect)
        {
            SetPosition(rect.X, rect.Y);
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, rect.Width), Math.Max(1, rect.Height));
        }

        public void SetSize(int w, int h)
        {
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, w), Math.Max(1, h));
        }

        public void SetSize(Point screenSize)
        {
            Display = new RenderTarget2D(GraphicsDevice, Math.Max(1, screenSize.X), Math.Max(1, screenSize.Y));
        }

        public void SetPosition(Vector2 pos) { Position = pos; }
        public void SetPosition(int x, int y) { Position = new Vector2(x, y); }



        // Drawing:
        public void SetAsRenderTarget() => GraphicsDevice.SetRenderTarget(Display);
        public void EndRenderTarget() => GraphicsDevice.SetRenderTarget(null);
        public void Clear(Color color) => GraphicsDevice.Clear(color);
        public void Clear() => GraphicsDevice.Clear(Color.Black);
    }
}
