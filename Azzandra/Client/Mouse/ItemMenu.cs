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
    public class ItemMenu : MouseItem
    {
        private readonly string Title;
        private readonly List<string> Options;
        private readonly string[] Desc;
        private readonly List<string> Info;

        //private readonly Rectangle SlotRegion;
        private readonly int Pad = 5, LineH = 16;

        private readonly Color BGColor = new Color(31, 31, 31), OutlineColor = new Color(127, 127, 127), InlineColor = new Color(63, 63, 63);

        private bool CanClose = false;
        private bool RequiresMouse = true;

        protected SpriteFont TitleFont = Assets.Gridfont, Font = Assets.Medifont;

        public ItemMenu(GameClient gameClient, Container container, int index, Vector2 pos, Rectangle slotRegion, Rectangle containerRegion, bool requiresMouse = true) : base(container, index, gameClient)
        {
            var item = container.GetItemByIndex(index);
            Title = item.GetNameNotNull().CapFirst() + ':';
            Options = item.GetOptions();
            Options.Add("Abort");
            Info = item.GetInfo();
            RequiresMouse = requiresMouse;

            int width = 14 * 16;
            Desc = Util.SeparateString(item.Desc, Font, width - 2 * Pad);
            int height = (Desc.Length + Info.Count + Options.Count + 1) * LineH + 3 * LineH / 2 + 2 * Pad;

            var screen = GameClient.DisplayHandler.Screen;
            int x = Math.Max(containerRegion.Left, Math.Min(containerRegion.Right - width, (int)pos.X - width / 2));
            int y = Math.Max(screen.Top, Math.Min(screen.Bottom - height - 16, (int)pos.Y));

            //SlotRegion = slotRegion;
            Surface.SetBounds(new Rectangle(x, y, width, height));
            Surface.Outline = true;
        }

        public override void Render()
        {
            if (Input.IsKeyPressed[Keys.Escape])
                Destroy();

            var region = Surface.Region;

            int pad = 4;
            if (RequiresMouse && !Input.MouseHover(region.X - pad, region.Y - pad, region.Width + 2 * pad, region.Height + 2 * pad))
            {
                // Kill self
                GameClient.DisplayHandler.MouseItem = null;
            }
            else
            {
                GraphicsDevice.SetRenderTarget(Surface.Display);
                GraphicsDevice.Clear(BGColor);
                SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                var format = new TextFormat(Color.White, TitleFont, Alignment.Centered);
                var textDrawer = new TextDrawer(new Vector2(region.Width / 2, LineH / 2 + Pad), LineH, format);

                // Draw title:
                textDrawer.DrawLine(Title);

                textDrawer.Format.Font = Font;
                textDrawer.Format.Alignment = Alignment.VCentered;
                textDrawer.SetPosition(Pad, (int)textDrawer.Pos.Y);
                textDrawer.DrawHorizontalBar(region.Width - 2 * Pad, 1, InlineColor);

                // Draw item desc:
                textDrawer.DefaultColor = Color.LightBlue;
                textDrawer.LastColor = textDrawer.DefaultColor;
                textDrawer.Format.Alignment = Alignment.Centered;
                textDrawer.SetPosition(region.Width / 2, (int)textDrawer.Pos.Y);
                foreach (var line in Desc)
                    textDrawer.DrawLine(line);

                textDrawer.DefaultColor = Color.White;
                textDrawer.LastColor = textDrawer.DefaultColor;
                textDrawer.Format.Alignment = Alignment.VCentered;
                textDrawer.SetPosition(Pad, (int)textDrawer.Pos.Y);

                textDrawer.DrawHorizontalBar(region.Width - 2 * Pad, 1, InlineColor);

                // Keyboard input for the last 2 options: "drop" and "abort":
                if (CanClose && GameClient.KeyboardFocus == GameClient.Focus.General)
                {
                    if (Input.IsKeyPressed[Keys.D9])
                    {
                        PerformOption("drop");
                        Destroy();
                    }
                    else if (Input.IsKeyPressed[Keys.D0])
                    {
                        PerformOption("abort");
                        Destroy();
                    }
                }

                // Draw item options
                for (int i = 0; i < Options.Count; i++)
                {
                    var line = Options[i];

                    // Keyboard input:
                    if (CanClose && GameClient.KeyboardFocus == GameClient.Focus.General && i < Options.Count - 2 && Input.IsKeyPressed[Util.IntToKey(i + 1)])
                    {
                        PerformOption(line.ToLower());
                        Destroy();
                    }

                    var hover = GameClient.DisplayHandler.IsHoverSurface(Surface) && 
                        Input.MouseHover(Surface.Position + new Vector2(0, textDrawer.Pos.Y - LineH / 2), new Vector2(region.Width, LineH));
                    if (hover)
                    {
                        //textDrawer.LastColor = Color.Aqua;
                        if (Input.IsMouseLeftPressed)
                        {
                            PerformOption(line.ToLower());
                        }
                    }

                    var colorStr = hover ? "<aqua>" : "<white>";
                    var numStr = i < Options.Count - 2 ? (i + 1) + "" : i == Options.Count - 2 ? "9" : "0";
                    textDrawer.DrawLine(colorStr + numStr + ". " + line.CapFirst() + "");
                    //if (hover) textDrawer.LastColor = Color.White;
                }

                textDrawer.DrawHorizontalBar(region.Width - 2 * Pad, 1, InlineColor);

                // Draw item info:
                textDrawer.DefaultColor = Color.SlateGray;
                textDrawer.LastColor = textDrawer.DefaultColor;
                foreach (var line in Info)
                    textDrawer.DrawLine(line);

                SpriteBatch.End();
                GraphicsDevice.SetRenderTarget(null);

            }

            // Kill self
            if ((Input.IsMouseLeftPressed || Input.IsMouseRightPressed) && CanClose)
                Destroy();
            CanClose = true;
        }

        private void PerformOption(string option)
        {
            if (option == "abort")
            {
                Destroy();
            }
            else
            {
                var item = Container?.GetItemByIndex(Index);
                if (item != null)
                {
                    GameClient.Server.SetPlayerAction(new ActionItem(GameClient.Server.User.Player, item, option));
                }
            }
        }
    }
}
