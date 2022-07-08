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
    public abstract class ChatInterface
    {
        protected GameClient GameClient;
        protected GraphicsDevice GraphicsDevice;
        protected SpriteBatch SpriteBatch;
        protected SpriteFont TitleFont = Assets.Gridfont, Font = Assets.Medifont;

        public Surface Surface { get; protected set; }

        public ChatInterface(GameClient gameClient)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;

            Surface = new Surface(GameClient.Engine);
            GameClient.KeyboardFocus = GameClient.Focus.TextInput;
            Surface.SetBounds(GameClient.DisplayHandler.LogSurface.Region);
        }

        public virtual void Update()
        {
            if (Input.IsKeyDown[Keys.Escape])
            {
                Close();
            }
        }

        public virtual void OnResize(Point screenSize)
        {
            Surface?.SetSize(screenSize);
        }

        public abstract void Render();

        public virtual void Close()
        {
            GameClient.DisplayHandler.ChatInterface = null;
            GameClient.KeyboardFocus = GameClient.Focus.General;
        }
    }
}
