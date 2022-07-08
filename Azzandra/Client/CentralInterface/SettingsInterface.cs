using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SettingsInterface : Interface
    {
        public override bool DisableControls => true;

        private SettingsRenderer SettingsRenderer;

        public SettingsInterface(GameClient gameClient) : base(gameClient)
        {
            SettingsRenderer = new SettingsRenderer(gameClient.Engine, gameClient.Engine.Settings);
            SettingsRenderer.SetBackButtonEffect(() => GameClient.DisplayHandler.Interface = new PausedInterface(gameClient));
        }

        public override void Render(GraphicsDevice gd, SpriteBatch sb)
        {
            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.White * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            SettingsRenderer.Render(Surface, gd, sb);

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public override void Destroy()
        {
            GameClient.Engine.SaveClientSettings();
        }
    }
}
