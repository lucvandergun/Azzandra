using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TabHandler
    {
        private readonly GameClient GameClient;
        private readonly GraphicsDevice GraphicsDevice;
        private readonly SpriteBatch SpriteBatch;

        //public enum TabID { Null, Inventory, Equipment, Stats }
        public string[] TabID = new string[] { "Inventory", "Equipment", "Spells" };
        public int CurrentTab;

        public InventoryInterface InventoryInterface { get; private set; }
        public EquipmentInterface EquipmentInterface { get; private set; }
        public StatsInterface StatsInterface { get; private set; }
        public SpellsInterface SpellsInterface { get; private set; }
        //public EnvironmentInterface EnvironmentInterface { get; private set; }


        private readonly SpriteFont Font = Assets.Gridfont;


        public TabHandler(GameClient gameClient)
        {
            GameClient = gameClient;
            GraphicsDevice = GameClient.Engine.GraphicsDevice;
            SpriteBatch = GameClient.Engine.SpriteBatch;
            CurrentTab = 0;

            //Create tabinterfaces
            InventoryInterface = new InventoryInterface(GameClient);
            EquipmentInterface = new EquipmentInterface(GameClient);
            StatsInterface = new StatsInterface(GameClient);
            SpellsInterface = new SpellsInterface(GameClient);
            //EnvironmentInterface = new EnvironmentInterface();
        }


        public void RenderMenu(Surface surface)
        {
            // Renders menu buttons on given surface.
            // Spaced from the left, centered on surface height.

            var isHoverSurface = GameClient.DisplayHandler.IsHoverSurface(surface);

            var region = surface.Region;
            int stringWidth;
            int xPadding = region.Height / 2;

            Vector2 buttonPos = new Vector2(surface.Width, 0);
            Vector2 buttonSize;
            Vector2 drawOffset = new Vector2(xPadding, surface.Height / 2);

            string tab, str;
            bool switchTab;
            var tabs = TabID.ToList();
            if (GameClient.Engine.Settings.DisplayInput)
                tabs.Add("Environment"); //.Insert(0, "Environment");

            for (int i = tabs.Count - 1; i >= 0; i--)
            {
                //retrieve button string
                tab = tabs[i];
                str = (i + 1) + ". " + tab;//"(" + (i + 1) + ") " + tab;

                //calculate button sizes
                stringWidth = Util.GetStringWidth(str, Font);
                buttonSize = new Vector2(stringWidth + 2 * xPadding, surface.Height);
                buttonPos.X -= buttonSize.X;

                //check whether tab should be switched
                switchTab = false;

                if (GameClient.KeyboardFocus == GameClient.Focus.General &&  Input.IsKeyPressed[Util.IntToKey(i + 1)])
                {
                    switchTab = true;
                }

                Color col;
                bool hover = isHoverSurface && Input.MouseHover(buttonPos + surface.Position, buttonSize);
                if (hover)
                {
                    if (Input.IsMouseLeftPressed)
                    {
                        switchTab = true;
                    }
                    col = Color.Aqua;
                    //Display.DrawInline(Display.MakeRectangle(buttonPos, buttonSize), Color.White);
                }
                else col = Color.White;

                //switch tab if user requests
                if (switchTab)
                {
                    if (CurrentTab != i)
                    {
                        CurrentTab = i;

                        //perform specific swich-to-tab actions here

                    }
                    //else CurrentTab = -1; // uncomment to enable showing no tab
                }

                if (i == CurrentTab) str += '*';
                Display.DrawStringVCentered((buttonPos + drawOffset), str.CapFirst(), Font, col);
            }
        }

        
        public void RenderTab(Surface surface)
        {
            switch (CurrentTab)
            {
                case 0:
                    InventoryInterface.Render(surface);
                    break;

                case 1:
                    EquipmentInterface.Render(surface);
                    break;

                //case 2:
                //    StatsInterface.Render(surface);
                //    break;

                case 2:
                    SpellsInterface.Render(surface);
                    break;

                case 3:
                    GameClient.DisplayHandler.EnvironmentInterface.Render(surface);
                    break;
            }
        }

        public virtual void OnResize(Point region)
        {
            InventoryInterface.OnResize(region);
            EquipmentInterface.OnResize(region);
            StatsInterface.OnResize(region);
            SpellsInterface.OnResize(region);
        }

    }
}
