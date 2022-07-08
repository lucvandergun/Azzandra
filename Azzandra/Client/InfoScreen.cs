//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra
//{
//    public class InfoScreen
//    {
//        public readonly Engine Engine;
//        public readonly GraphicsDevice GraphicsDevice;
//        public readonly SpriteBatch SpriteBatch;
//        public Surface Surface { get; protected set; } = new Surface();
//        public static SpriteFont Font = Assets.Medifont, TitleFont = Assets.Gridfont;

//        private int TextLength => Surface.Width - (Surface.Height - 2 * 8) - 8 - 16;

//        public string Title;
//        public Texture2D Asset;
//        public string[] Text;
//        public bool IsItem;

//        public InfoScreen(Item item, Engine engine)
//        {
//            Engine = engine;
//            GraphicsDevice = Engine.GraphicsDevice;
//            SpriteBatch = Engine.SpriteBatch;

//            SetSize();

//            Title = item.ToString().CapitalizeFirst();
//            Asset = Assets.GetSprite(item.Asset);
//            Text = Util.SeparateString(item.GetInfo(), TextLength);

//            IsItem = true;

//            SetInfoScreen(this);
//        }

//        public InfoScreen(Instance inst)
//        {
//            SetSize();

//            Title = inst.ToString().CapitalizeFirst();
//            Asset = null;
//            Text = Util.SeparateString(inst.GetInfo(), TextLength);

//            SetInfoScreen(this);
//        }

//        public static void SetInfoScreen(InfoScreen item)
//        {
//            Engine.DisplayHandler.InfoScreen = item;
//        }

//        protected void SetSize()
//        {
//            Surface.SetSize(Engine.DisplayHandler.LogSurface.Region);
//        }



//        public virtual void Render(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
//        {
//            var surface = Surface;

//            surface.SetAsRenderTarget();
//            surface.Clear();
//            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);


//            //draw border
//            int xpad = 8, ypad = 12;
//            var rect = new Rectangle(xpad, ypad, surface.Width - 2 * xpad, surface.Height - 2 * ypad);

//            Display.DrawRect(rect, new Color(31, 31, 31));
//            Display.DrawOutline(rect, new Color(63, 63, 63));


//            //draw asset
//            if (Asset != null)
//            {
//                var assetPos = new Vector2(Surface.Height / 2) + new Vector2(8, 0);
//                int scale = 3;
//                Display.DrawTexture(assetPos - new Vector2(Asset.Width, Asset.Height) * scale / 2, Asset, scale);
//            }

//            //draw and handle close button
//            var areaSize = new Vector2(Surface.Height);
//            var hover = DisplayHandler.IsHoverSurface(Surface) && Input.MouseHover(Surface.Position + Surface.Size - areaSize, areaSize);
//            var color = hover ? Color.White : Color.Red;

//            int strOffset = 12;
//            var strPos = new Vector2(rect.Right - strOffset, rect.Top + strOffset);
//            Display.DrawStringCentered(strPos, "x", TitleFont, color);

//            if (hover && Input.IsMouseLeftPressed)
//            {
//                Close();

//                //deselect slot
//                if (IsItem)
//                {
//                    /*
//                    if (Engine.DisplayHandler.Interface is InventoryInterface ir)
//                    {
//                        ir.SelectedSlot = null;
//                    }
//                    */
//                }
//            }


//            //calculate dimensions
//            int amtOfLines = Text.Length + 1; // +1 for title
//            var startOffset = new Vector2(Surface.Height + 8, (Surface.Height - (amtOfLines - 1) * 16) / 2);
//            var stringOffset = new Vector2(0, 16);

//            //draw title
//            Display.DrawStringVCentered(startOffset, Title, TitleFont);

//            //draw lines
//            for (int i = 0; i < Text.Length; i++)
//            {
//                var pos = startOffset + (i + 1) * stringOffset;
//                Display.DrawStringVCentered(pos, Text[i], Font);
//            }


//            SpriteBatch.End();
//            surface.EndRenderTarget();
//        }

//        public void Close()
//        {
//            Engine.DisplayHandler.InfoScreen = null;
//        }
//        public virtual void Destroy()
//        {
            
//        }

        
//    }
//}
