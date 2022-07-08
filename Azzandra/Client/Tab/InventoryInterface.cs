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
        private Item HoverSlot;

        public InventoryInterface(GameClient gameClient) : base(gameClient)
        {
            
        }

        protected override void DrawAdditional(Surface surface)
        {
            // Draw tooltip
            DrawToolTip(surface, HoverSlot);
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

            HoverSlot = null;

            // Render item slots
            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotSize = new Vector2(surface.Width, slotHeight);
            var slotOffset = new Vector2(0, slotHeight);
            var stringOffset = new Vector2(4, (slotHeight - Util.GetFontHeight(Font)) / 2);
            var nameOffset = new Vector2(0, 0);//32

            // Update area scroll offset:
            SubArea.FullRenderSize = new Vector2(surface.Width, slotHeight * Inventory.Items.Count);
            if (isHoverSurface) SubArea.UpdateScrollPosition();
            startOffset -= SubArea.RenderOffset;


            //render slots
            for (int i = 0; i < Inventory.Items.Count; i++)
            {
                //Get current drag item
                var dragItem = GameClient.DisplayHandler.MouseItem as DragItem;

                //Leave null if drag item doesn't have inventory as container
                if (dragItem != null)
                    dragItem = (dragItem.Container != Inventory) ? null : dragItem;

                //Get item data
                var item = Inventory.Items[i];
                var name = item?.ToString().CapFirst();
                var options = item?.GetOptions();


                //Draw position:
                Vector2 pos = startOffset + slotOffset * i;

                //Determine whether is hovered slot
                bool hover = isHoverSurface && Input.MouseHover(absoluteSurfacePos + pos, slotSize);


                //if this slot isnt drag slot
                bool isDragIndex = (dragItem?.Index == i) ? true : false;
                if (!isDragIndex)
                {
                    //if hover over slot
                    if (hover)
                    {
                        //if no drag slot exists
                        if (dragItem == null)
                        {
                            if (item != null)
                            {
                                //set drag slot
                                if (Input.IsMouseLeftPressed)
                                {
                                    Vector2 clickPos = Input.MousePosition;
                                    Vector2 clickOffset = clickPos - (absoluteSurfacePos + pos + stringOffset + nameOffset);
                                    new DragItem(Inventory, i, clickPos, clickOffset, GameClient);
                                }

                                //open up slot menu
                                else if (Input.IsMouseRightPressed)
                                {
                                    var m = new ItemMenu(GameClient, Inventory, i, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion);
                                }
                            }
                        }
                        else
                        {
                            // Swap slot with drag slot
                            if (Input.IsMouseLeftReleased)
                            {
                                // Insert held slot at hover position
                                Inventory.MoveItem(Inventory.GetItemByIndex(dragItem.Index), i);
                            }
                        }

                        HoverSlot = item;

                    }

                    // Draw slot string
                    //if (i < KeyInputs.Length)
                    //    Display.DrawString(pos + stringOffset + nameOffset / 4, KeyInputs[i].ToString() + " -", Font, Color.SlateGray);

                    Display.DrawString(pos + stringOffset + nameOffset, name, Font, item.StringColor);
                }
                else
                {
                    // For drag item: -> Perform default option if mouse release without mouse movement
                    if (Input.IsMouseLeftReleased)
                    {
                        if (dragItem.ClickPos.Equals(Input.MousePosition))
                        {
                            if (options.Count > 0)
                                GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, item.GetDefaultOption()));

                            dragItem.Destroy();
                        }
                    }
                }

                //draw hover slot bounds
                if (hover)
                {
                    Display.DrawInline(Display.MakeRectangle(pos, slotSize), Color.White);
                }
            }

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
