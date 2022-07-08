using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public interface IGameCreationStage
    {
        void Update(GameTime gameTime);

        void Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb, Surface surface);

        bool CanContinue();
    }
}
