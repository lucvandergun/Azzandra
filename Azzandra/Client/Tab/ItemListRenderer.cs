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
    class ItemListRenderer
    {
        private readonly GameClient GameClient;
        public Func<Container> Container;
        private readonly bool AllowDrag;
        public Item HoverSlot { get; private set; }
        private Item ClickedSlot;
        private readonly SpriteFont Font = Assets.Medifont;
        private readonly TextFormat Format = new TextFormat(Color.White, Assets.Medifont, Alignment.VCentered, false);
        public Func<int, string> NoItem = (i) => "";

        public ItemListRenderer(GameClient gameClient, Func<Container> container, bool allowDrag = false)
        {
            GameClient = gameClient;
            Container = container;
            AllowDrag = allowDrag;
        }

        public void Render(Vector2 absoluteSurfacePos, Rectangle outerRegion, Vector2 startOffset, Vector2 slotSize, bool isHoverSurface)
        {
            var items = Container.Invoke()?.GetItems();
            if (items == null)
                return;

            HoverSlot = null;

            // Render item slots
            var slotOffset = new Vector2(0, slotSize.Y);
            var stringOffset = new Vector2(4, slotSize.Y / 2);
            var nameOffset = new Vector2(13, 0);//32

            for (int i = 0; i < items.Count(); i++)
            {
                var item = items.ElementAt(i);

                //Get current drag item
                var dragItem = AllowDrag
                    ? GameClient.DisplayHandler.MouseInterface as DragItem
                    : null;

                ////Leave null if drag item doesn't have inventory as container
                //if (dragItem != null)
                //    dragItem = (dragItem.Container != Inventory) ? null : dragItem;

                //Get item data
                var name = item?.ToString().CapFirst();

                //Draw position:
                Vector2 pos = startOffset + slotOffset * i;

                // Keyboard Handling:
                if (item != null && GameClient.KeyboardFocus == GameClient.Focus.General && Input.IsKeyPressed[Input.GetKeyBind(i)] && GameClient.DisplayHandler.MouseInterface == null)
                {
                    // Perform default option directly with SHIFT:
                    if (Input.IsKeyDown[Keys.LeftShift] || Input.IsKeyDown[Keys.RightShift])
                    {
                        PerformDefaultOption(item);
                    }
                    // Open up item menu otherwise:
                    else
                    {
                        new ItemMenu(GameClient, item.Container, i, absoluteSurfacePos + pos + slotOffset, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion, false);
                    }
                }

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
                                    ClickedSlot = item;

                                    if (AllowDrag)
                                    {
                                        Vector2 clickPos = Input.MousePosition;
                                        Vector2 clickOffset = clickPos - (absoluteSurfacePos + pos + stringOffset + nameOffset) + new Vector2(0, Util.GetFontHeight(Font) / 2);
                                        new DragItem(item.Container, i, clickPos, clickOffset, GameClient);
                                    }
                                }

                                //open up slot menu
                                else if (Input.IsMouseRightPressed)
                                {
                                    new ItemMenu(GameClient, item.Container, i, Input.MousePosition, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion);
                                }
                            }
                        }
                        else
                        {
                            // Swap slot with drag slot
                            if (Input.IsMouseLeftReleased && Container.Invoke() is Inventory inventory)
                            {
                                // Insert held slot at hover position
                                inventory.MoveItem(inventory.GetItemByIndex(dragItem.Index), i);
                            }
                        }

                        HoverSlot = item;

                    }

                    // Draw slot string
                    //if (i < KeyInputs.Length)
                    //    Display.DrawString(pos + stringOffset + nameOffset / 4, KeyInputs[i].ToString() + " -", Font, Color.SlateGray);

                    if (item != null)
                    {
                        TextFormatter.DrawString(pos + stringOffset, Input.GetKeyBindString(i) + ". ", Format, Color.LightSlateGray);
                        TextFormatter.DrawString(pos + stringOffset + nameOffset, name, Format, item.StringColor);
                    }
                    else if (NoItem != null)
                    {
                        TextFormatter.DrawString(pos + stringOffset, Input.GetKeyBindString(i) + ". ", Format, Color.LightSlateGray * 0.5f);
                        TextFormatter.DrawString(pos + stringOffset + nameOffset, NoItem.Invoke(i), Format, Color.LightSlateGray * 0.5f);
                    }
                }

                if ((isDragIndex || !AllowDrag) && hover)
                {
                    // For drag item: -> Perform default option if mouse release without mouse movement
                    if (Input.IsMouseLeftReleased && ClickedSlot == item)
                    {
                        if (AllowDrag)
                        {
                            if (dragItem.ClickPos.Equals(Input.MousePosition))
                            {
                                PerformDefaultOption(item);
                                dragItem.Destroy();
                            }
                        }
                        else
                        {
                            PerformDefaultOption(item);
                        }
                    }
                }

                //draw hover slot bounds
                if (hover && item != null)
                {
                    Display.DrawInline(Display.MakeRectangle(pos, slotSize), Color.White);
                }
            }
        }

        private void PerformDefaultOption(Item item)
        {
            if (item == null)
                return;
            var defaultOption = item.GetDefaultOption();
            if (defaultOption != null)
                GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, defaultOption));
        }
    }
}
