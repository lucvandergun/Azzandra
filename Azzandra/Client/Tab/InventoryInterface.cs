using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Azzandra
{
    public class InventoryInterface : TabInterface
    {
        private string QtyStr => Inventory != null ? "(" + Inventory.Items.Count + "/" + Inventory.MaxItems + ") " : "";
        protected override string Title => "= Inventory " + QtyStr + "=";
        public Inventory Inventory => GameClient.Server?.User.Inventory ?? null;

        private ItemListRenderer ItemListRenderer;

        public InventoryInterface(GameClient gameClient) : base(gameClient)
        {
            ItemListRenderer = new ItemListRenderer(gameClient, () => Inventory, true);
        }

        protected override void DrawAdditional(Surface surface)
        {
            // Draw tooltip
            DrawToolTip(surface, ItemListRenderer.HoverSlot);
        }

        protected override void RenderSubArea(Rectangle outerRegion, bool isHoverSurface)
        {
            var surface = SubArea;
            var absoluteSurfacePos = new Vector2(outerRegion.X, outerRegion.Y) + surface.Position;

            GameClient.Engine.GraphicsDevice.SetRenderTarget(surface.Display);
            GameClient.Engine.GraphicsDevice.Clear(Color.Black);
            GameClient.Engine.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            if (Inventory == null)
                return;

            // Render item slots
            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotSize = new Vector2(surface.Width, slotHeight);

            // Update area scroll offset:
            SubArea.FullRenderSize = new Vector2(surface.Width, slotHeight * Inventory.Items.Count);
            if (isHoverSurface) SubArea.UpdateScrollPosition();
            startOffset -= SubArea.RenderOffset;

            ItemListRenderer.Render(absoluteSurfacePos, outerRegion, startOffset, slotSize, isHoverSurface);


            GameClient.Engine.SpriteBatch.End();
            GameClient.Engine.GraphicsDevice.SetRenderTarget(null);
        }


        /// <summary>
        /// Creates a tooltip string based on current hover slot
        /// </summary>
        /// <param name="pos"></param>
        private void DrawToolTip(Surface surface, Item item)
        {
            //Return if no hover slot
            if (item == null)
                return;

            //Get item action & name
            string action = item.GetDefaultOption();
            string name = item.QuantityName(item.Quantity);

            //Capitalize
            if (action == null) name = name.CapFirst();
            else action = action.CapFirst();

            //Start drawing
            int titleLength = Util.GetStringWidth(Title, TitleFont);
            var pos = TitleOffset + new Vector2(titleLength + 16, -1);

            var textDrawer = new TextDrawer(pos, 16, Alignment.VCentered, Font, Color.White);

            if (action != null)
                textDrawer.Draw("<aqua>" + action + " <r>");

            textDrawer.Draw(name);
        }
    }
}
