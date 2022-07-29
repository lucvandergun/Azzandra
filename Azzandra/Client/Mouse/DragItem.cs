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
    public class DragItem : MouseItem
    {
        public readonly Vector2 ClickPos;
        public readonly Vector2 ClickOffset;

        private string Name;
        private SpriteFont Font = Assets.Medifont;
        private Vector2 Pos;

        public DragItem(Container container, int index, Vector2 clickPos, Vector2 clickOffset, GameClient gameClient) : base(container, index, gameClient)
        {
            ClickPos = clickPos;
            ClickOffset = clickOffset;

            //get slot data
            var item = Container.GetItemByIndex(Index);
            if (item == null)
                Destroy();

            Name = item.ToString().CapFirst();

            int width = Util.GetStringWidth(Name, Font) + 4;
            Pos = new Vector2(0, Util.GetFontHeight(Font) / 2);


            //Specify surface properties
            Surface.SetBounds(new Rectangle(0, 0, width, Util.GetFontHeight(Font)));
            Surface.CanHover = false;
            Surface.Outline = false;
        }

        public override void Render()
        {
            if (Input.IsKeyPressed[Keys.Escape])
                Destroy();

            //reposition surface to mouse pos
            Surface.SetPosition(Input.MousePosition - ClickOffset);

            //start
            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.Black * 0f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            var item = Container.GetItemByIndex(Index);
            if (item == null)
                Destroy();


            Color color = Color.White * 0.75f;
            Display.DrawStringVCentered(Pos, Name, Font, color);


            //click release over world view
            if (Input.IsMouseLeftReleased)
            {
                var option = DiscardOption(item);
                if (option != null)
                {
                    GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, option));
                }

                Destroy();
            }



            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        private string DiscardOption(Item item)
        {
            if (GameClient.DisplayHandler.IsHoverSurface(GameClient.DisplayHandler.MenuSurface))
                return null;

            /*
            var mainInterface = Engine.DisplayHandler.CurrentMainInterface;
            if (mainInterface != null)
            {
                if (DisplayHandler.IsHoverSurface(Engine.DisplayHandler.CurrentMainInterface.Surface))
                {
                    if (mainInterface is ShopInterface)
                    {
                        if (item.Quantity > 1)
                            return "sell all";
                        else
                            return "sell 1";
                    }
                }
            }
            */

            return "drop";
        }
    }
}
