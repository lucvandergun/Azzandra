using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Interface
    {
        public readonly GameClient GameClient;
        public readonly GraphicsDevice GraphicsDevice;
        public readonly SpriteBatch SpriteBatch;
        public Surface Surface { get; protected set; }

        public static SpriteFont Font = Assets.Medifont, TitleFont = Assets.Gridfont;
        public bool AllowClose { get; protected set; } = false; // used to counter same frame opening and closing.
        public virtual bool CanBeClosed => true;
        public virtual bool DisableControls => false;

        public Interface(GameClient gameClient)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;

            Surface = new Surface(GameClient.Engine);
            OnResize(gameClient.DisplayHandler.Screen.Size);

            if (DisableControls)
                GameClient.KeyboardFocus = GameClient.Focus.Interface;
        }

        public virtual void Update()
        {
            if (CanBeClosed && AllowClose && Input.IsKeyPressed[Keys.Escape])
            {
                Close();
            }

            if (!AllowClose)
                AllowClose = true;
        }

        public virtual void OnResize(Point screenSize)
        {
            Surface?.SetSize(screenSize);
        }

        public abstract void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);

        public void Close()
        {
            GameClient.DisplayHandler.Interface = null;
            GameClient.KeyboardFocus = GameClient.Focus.General;
            Destroy();
        }

        public abstract void Destroy();
    }
}
