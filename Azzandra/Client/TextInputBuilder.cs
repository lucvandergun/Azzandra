using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TextInputBuilder
    {
        public Engine Engine { get; protected set; }
        protected virtual char[] AllowedCharacters { get; }
        public SpriteFont Font;

        protected readonly string Cursor = "|";
        public int CursorPosition = 0;
        protected const int CursorSpeed = 60;
        protected int CursorTimer = 0;

        protected StringBuilder Builder = new StringBuilder();
        public string GetText() => Builder.ToString();
        public bool HasText() => Builder.Length > 0;
        public string DefaultText { get; set; }
        public int MaxLength { get; set; } = 12;

        private bool FirstFrame = false;


        public Action Send;
        //public Func<bool> IsFocussed;

        public bool IsFocussed { get; private set; }

        public TextInputBuilder(Engine engine, SpriteFont font, string startingText = null, string defaultText = null)
        {
            Engine = engine;
            Font = font;
            if (startingText != null)
            {
                Builder.Append(startingText);
                CursorPosition = Builder.Length;
            } 
            DefaultText = defaultText;
        }


        /// <summary>
        /// Sets IsFocussed to true and makes the game send all incomming keyboard input to this input builder.
        /// </summary>
        public void Enable()
        {
            Engine.Window.TextInput += GetTextInput;
            IsFocussed = true;
            CursorTimer = CursorSpeed / 3;
            FirstFrame = true;
        }
        /// <summary>
        /// Sets IsFocussed to false and returns all incomming keyboard input back to the game.
        /// </summary>
        public void Disable()
        {
            Engine.Window.TextInput -= GetTextInput;
            IsFocussed = false;
        }

        public void Update()
        {
            if (!FirstFrame && Input.IsKeyPressed[Keys.Enter])
            {
                Send?.Invoke();
                Disable();
            }

            if (FirstFrame)
                FirstFrame = false;

            if (CursorTimer > 0) CursorTimer--;
            else CursorTimer = CursorSpeed;
        }

        public void GetTextInput(object sender, TextInputEventArgs e)
        {
            //check if character is allowed according to text input variant
            if (AllowedCharacters != null && e.Character != '\b')
                if (!AllowedCharacters.Contains(e.Character))
                    return;

            //if ((charValue >= 'A') && (charValue <= 'Z'))

            Debug.WriteLine("key: "+ e.Key);

            //backspace
            if (e.Character == '\b')
            {
                if (CursorPosition > 0 && CursorPosition <= Builder.Length) // distinct 'if'!
                {
                    Builder.Remove(CursorPosition - 1, 1);
                    CursorPosition--;
                }
            }

            //// Cursor move
            //else if(Input.IsKeyDown[Keys.Left])
            //{
            //    if (CursorPosition > 0) // distinct 'if'!
            //    {
            //        CursorPosition = Math.Max(CursorPosition - 1, 0);
            //    }
            //}
            //else if (Input.IsKeyDown[Keys.Right])
            //{
            //    if (CursorPosition < Builder.Length) // distinct 'if'!
            //    {
            //        CursorPosition = Math.Min(CursorPosition + 1, Builder.Length);
            //    }
            //}

            //add character to builder
            else if (Font.Characters.Contains(e.Character) && Builder.Length < MaxLength)
            {
                Builder.Append(e.Character);
                CursorPosition++;
            }
        }

        /// <summary>
        /// Renders the current stringbuilder along with the cursor at the given position.
        /// </summary>
        /// <param name="pos">The position to render at.</param>
        /// <param name="centered">Whether to center the string at the x coordinate. Otherwise from left.</param>
        /// <param name="color">The color to render the text in.</param>
        public void Render(Vector2 pos, bool centered, Color color, bool renderDefaultText)
        {
            // Update cursor display
            Update();
            
            var text = HasText() ? GetText() : renderDefaultText && DefaultText != null ? DefaultText : "";
            int w = Util.GetStringWidth(text, Font) / 2;
            if (!centered) // Relocate pos if not centered.
                pos.X += w;

            int cursorOffset = Util.GetStringWidth(text.Substring(0, CursorPosition), Font) - w;

            // Draw string & cursor
            Display.DrawStringCentered(pos, text, Font, color);
            if (IsFocussed && CursorTimer <= CursorSpeed / 2)
                Display.DrawStringCentered(pos + new Vector2(cursorOffset, 0), Cursor, Font, color);
        }
    }
}
