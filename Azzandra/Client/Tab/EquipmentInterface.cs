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
    public class EquipmentInterface : TabInterface
    {
        public readonly List<string> SlotID = new List<string>() { "weapon", "shield", "helm", "body", "legs", "boots", "gloves", "amulet", "ring", "cape", "ammunition", "spell" };

        protected override string Title => "= Equipment =";
        public Equipment Equipment => GameClient.Server?.User.Equipment ?? null;
        private Item HoverSlot;
        private int ClickedSlot = -1;

        public EquipmentInterface(GameClient gameClient) : base(gameClient)
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

            if (Equipment == null)
                return;

            HoverSlot = null;

            //Render item slots
            int slotHeight = 20;
            var startOffset = new Vector2(0, 0);
            var slotSize = new Vector2(surface.Width / 2, slotHeight);
            var slotOffset = new Vector2(0, slotHeight);
            var iconOffset = (new Vector2(slotHeight) - new Vector2(16)) / 2;
            var stringOffset = new Vector2(slotHeight + 4, (slotHeight - Util.GetFontHeight(Font)) / 2);

            // Update area scroll offset:
            SubArea.FullRenderSize = new Vector2(surface.Width, slotHeight * (Equipment.Items.Length + 2));
            if (isHoverSurface) SubArea.UpdateScrollPosition();
            startOffset -= SubArea.RenderOffset;


            //render slots
            for (int i = 0; i < Equipment.Items.Length; i++)
            {
                // Get current drag item
                var dragItem = GameClient.DisplayHandler.MouseItem as DragItem;

                // Leave null if drag item doesn't have inventory as container
                if (dragItem != null)
                    dragItem = (dragItem.Container != Equipment) ? null : dragItem;

                // Get item data
                var item = Equipment.Items[i];
                var name = item?.ToString().CapFirst();
                var options = item?.GetOptions();
                

                // Draw position:
                Vector2 pos = startOffset + slotOffset * i;

                // Determine whether is hovered slot
                bool hover = isHoverSurface && Input.MouseHover(absoluteSurfacePos + pos, slotSize);


                // Check if slot has an item
                if (item != null)
                {
                    // Hover over slot
                    if (hover)
                    {
                        // Perform default action
                        if (Input.IsMouseLeftPressed)
                        {
                            ClickedSlot = i;
                        }
                        else if (ClickedSlot == i && Input.IsMouseLeftReleased)
                        {
                            GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, item.GetDefaultOption()));
                        }

                        // Open up slot menu
                        else if (Input.IsMouseRightPressed)
                        {
                            new ItemMenu(GameClient, Equipment, i, Display.MakeRectangle(absoluteSurfacePos + pos, slotSize), outerRegion);
                        }

                        HoverSlot = item;

                        // Draw hover slot bounds
                        Display.DrawInline(Display.MakeRectangle(pos, slotSize), Color.White);
                    }

                    // Draw item string
                    Display.DrawString(pos + stringOffset, name, Font, item.StringColor);
                }
                else
                {
                    // Draw empty slot string
                    var str = "No " + SlotID[i];
                    Display.DrawString(pos + stringOffset, str, Font, Color.LightSlateGray * 0.5f);
                }


                // Draw slot icon
                var iconColor = item != null ? Color.White : Color.LightSlateGray * 0.5f;
                Display.DrawTexture(pos + iconOffset, Assets.EquipmentIcons[i], iconColor);
            }

            RenderStats(surface, startOffset + new Vector2(surface.Width / 2, 0));

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



        private void RenderStats(Surface surface, Vector2 startOffset)
        {
            int slotHeight = 20;
            var stringOffset = new Vector2(4, (slotHeight - Util.GetFontHeight(Font)) / 2);

            var text = new TextDrawer(0, 0, slotHeight, Alignment.TopLeft, Font, Color.White);
            text.ResetColorOnCall = true;

            //draw equipment stats
            var pos = (startOffset + stringOffset).ToInt();
            text.SetPosition(pos.X, pos.Y);
            text.Font = TitleFont;
            text.DrawLine("Bonusses:");

            text.Font = Font;
            text.DefaultColor = Color.White;

            text.DrawLine("<slate>Accuracy:<r> " + Equipment.Accuracy);
            text.DrawLine("<slate>Damage:<r> " + Equipment.Damage);
            text.DrawLine("<slate>Spellcast:<r> " + Equipment.Spellcast);
            text.DrawLine("<slate>Evade:<r> " + Equipment.Evade);
            text.DrawLine("<slate>Parry:<r> " + Equipment.Parry, Color.Black, Equipment.CanParry() ? 0.0f : 0.5f);
            text.DrawLine("<slate>Block:<r> " + Equipment.Block, Color.Black, Equipment.CanBlock() ? 0.0f : 0.5f);
            text.DrawLine("<slate>Armour:<r> " + Equipment.Armour);
            text.DrawLine("<slate>Resistance:<r> " + Equipment.Resistance);
        }
    }
}
