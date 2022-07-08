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
    public abstract class TextInput : ChatInterface
    {
        protected readonly string Title;
        protected virtual char[] AllowedCharacters { get; }

        protected readonly string Cursor = "|";
        protected const int CursorSpeed = 60;
        protected int CursorTimer = 0;

        protected StringBuilder Builder = new StringBuilder();

        public TextInput(GameClient gameClient, string title) : base(gameClient)
        {
            //set title
            Title = title ?? "Null";

            //add text input handler to client
            GameClient.Engine.Window.TextInput += GetTextInput;
        }

        public override void Close()
        {
            base.Close();
            GameClient.Engine.Window.TextInput -= GetTextInput;
        }

        public override void Update()
        {
            if (Input.IsKeyPressed[Keys.Enter])
            {
                Send();
                Close();
            }

            if (CursorTimer > 0) CursorTimer--;
            else CursorTimer = CursorSpeed;
        }

        protected abstract void Send();

        public override void Render()
        {
            var region = Surface.Region;

            GraphicsDevice.SetRenderTarget(Surface.Display);
            GraphicsDevice.Clear(Color.Black * 0.75f);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            int lh, x, y;
            lh = Util.GetFontHeight(Font);
            y = (region.Height - lh * 2) / 2;

            //draw title
            x = (region.Width - Util.GetStringWidth(Title, Font)) / 2;
            Display.DrawString(x, y, Title, Font, Color.White);

            //draw string
            var text = Builder.ToString();
            int w = Util.GetStringWidth(text, Font);
            x = (region.Width - (w + Util.GetStringWidth(Cursor, Font))) / 2;
            Display.DrawString(x, y + lh, text, Font);

            //draw cursor
            if (CursorTimer > CursorSpeed / 2) Display.DrawString(x + w, y + lh, Cursor, Font);

            SpriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }



        public void GetTextInput(object sender, TextInputEventArgs e)
        {
            //check if character is allowed according to text input variant
            if (AllowedCharacters != null && e.Character != '\b')
                if (!AllowedCharacters.Contains(e.Character))
                    return;

            //backspace
            if (e.Character == '\b')
            {
                if (Builder.Length > 0)
                {
                    Builder.Remove(Builder.Length - 1, 1);
                }
            }

            //add character to builder
            else if (Font.Characters.Contains(e.Character))
            {
                Builder.Append(e.Character);
            }
        }


        protected uint GetItemQuantity(string str)
        {
            if (str == null) return 1;

            //read modifier (x1000)
            int mod = 0;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                char c = str[i];

                if (c == 'k') mod += 1;
                else if (c == 'm') mod += 2;
                else if (c == 'b') mod += 3;
                else
                {
                    break;
                }

                str = str.Remove(i);
            }

            if (!uint.TryParse(str, out var j))
                return 1;

            uint qty = j;
            for (int i = 0; i < mod && i < 4; i++)
            {
                var newQty = qty * 1000;
                if (newQty < qty) return uint.MaxValue;
                qty = newQty;
            }

            return qty;
        }
    }
}
