using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class MouseItem
    {
        protected GameClient GameClient;
        protected GraphicsDevice GraphicsDevice;
        protected SpriteBatch SpriteBatch;

        public readonly Container Container;
        public readonly int Index;

        public Surface Surface { get; protected set; }

        public MouseItem(Container container, int index, GameClient gameClient)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;

            //set this as current mouse item
            GameClient.DisplayHandler.MouseItem = this;

            Surface = new Surface(GameClient.Engine);
            Container = container;
            Index = index;
        }

        public abstract void Render();

        public virtual void Destroy()
        {
            GameClient.DisplayHandler.MouseItem = null;
        }
    }
}
