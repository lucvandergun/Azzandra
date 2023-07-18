using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ItemMenuOld : MouseItem
    {
        private readonly string Title;
        private readonly List<string> Options;

        protected SpriteFont Font = Assets.Medifont;

        public ItemMenuOld(Container container, int index, string title, List<string> options, Rectangle region, GameClient gameClient) : base(container, index, gameClient)
        {
            Title = title.CapFirst() + ':';
            Options = options;

            int x, y, w, h;
            w = 8 + Math.Max(Util.GetStringWidth(Title, Font), Util.GetMaxStringWidth(options, Font));
            h = (Options.Count + 1) * 16 + 8;
            x = Math.Max(region.Left, Math.Min(region.Right - w, (int)Input.MousePosition.X - w / 2));
            y = Math.Max(region.Top, Math.Min(region.Bottom - h, (int)Input.MousePosition.Y));

            Surface.SetBounds(new Rectangle(x, y, w, h));
            Surface.Outline = true;
        }

        public override void Render()
        {
            var region = Surface.Region;

            if (!Input.MouseHover(region.X, region.Y, region.Width, region.Height))
            {
                //kill self
                GameClient.DisplayHandler.MouseInterface = null;
            }
            else
            {
                GraphicsDevice.SetRenderTarget(Surface.Display);
                GraphicsDevice.Clear(Color.Black);
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                int offset = 4;
                int line = 0;
                int lineH = 16;

                Display.DrawString(offset, offset + line++ * lineH, Title, Font, new Color(191, 191, 191));

                for (int i = 0; i < Options.Count; i++)
                {
                    var option = Options[i];
                    Color col;
                    if (Input.MouseHover(region.X, region.Y + line * lineH, region.Width, lineH))
                    {
                        if (Input.IsMouseLeftPressed)
                        {
                            var item = Container.GetItemByIndex(Index);
                            if (item != null)
                            {
                                GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, Options[i]));
                            }
                        }
                        col = Color.Aqua;
                    }
                    else col = Color.White;

                    Display.DrawString(offset, offset + line++ * lineH, option.CapFirst(), Font, col);
                }

                SpriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);

                //kill self
                if (Input.IsMouseLeftPressed)
                {
                    GameClient.DisplayHandler.MouseInterface = null;
                }
            }
        }
    }
}
