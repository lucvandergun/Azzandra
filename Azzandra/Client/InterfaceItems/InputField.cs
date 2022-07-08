using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class InputField : InterfaceItem
    {
        public Vector2 Size { get; set; }
        public string DefaultText { get; set; }
        public string DefaultReturnText { get; set; }
        public Action OnEnter { get; set; }                     // Method to perform when the user presses enter.
        public void Receive()
        {
            SetUnFocussed();
            OnEnter?.Invoke();
        }
        
        private readonly TextInputBuilder InputBuilder;
        public string GetText() => InputBuilder.HasText() ? InputBuilder.GetText() : DefaultReturnText;
        public Color TextColor { get; set; } = Color.White;
        private Color DimTextColor = Color.Gray;


        public override void OnEnterKey()
        {
            OnEnter?.Invoke();
            OnTabKey();
        }

        // Whether it currently has all the keyboard input reserved.
        public override void SetFocussed()
        {
            if (!IsFocussed)
            {
                InputBuilder.Enable();
                IsFocussed = true;
            }
        }
        public override void SetUnFocussed()
        {
            if (IsFocussed)
            {
                InputBuilder.Disable();
                IsFocussed = false;
            }
        }
        public Func<bool> CanInteract { get; set; }             // Whether it can be selected and typed in.

        public InputField(Engine engine, Vector2 size, string defaultReturnText, string defaultText = null)
        {
            Size = size;
            DefaultText = defaultText;
            DefaultReturnText = defaultReturnText;
            InputBuilder = new TextInputBuilder(engine, Assets.Medifont, null, defaultText)
            {
                Send = new Action(Receive)
            };
        }

        /// <summary>
        /// Renders the field at the specified position in the surface.
        /// Invokes the OnEnter method when enter is typed.
        /// </summary>
        /// <param name="surface">The surface the field is drawn in.</param>
        /// <param name="pos">The relative position inside the surface to draw the input field centered.</param>
        /// <param name="gd">The GraphicsDevice</param>
        /// <param name="sb">The SpriteBatch</param>
        /// <param name="canHover">Whether the field can be hovered.</param>
        public void Render(Surface surface, Vector2 pos, GraphicsDevice gd, SpriteBatch sb, bool canHover)
        {
            bool canInteract = CanInteract == null || CanInteract();

            if (canInteract && IsFocussed) UpdateKeyInput();


            bool hover = Input.MouseHover(surface.Position + pos - Size/2, Size) && canHover;
            var textColor = canInteract && InputBuilder.HasText() ? TextColor : DimTextColor;
            var boundsColor = IsFocussed ? new Color(191, 191, 191) : new Color(63, 63, 63);

            // Draw outer rectangle bounds
            var rect = new Rectangle((pos - Size / 2).ToPoint(), Size.ToPoint());
            Display.DrawRect(rect, new Color(15, 15, 15));
            Display.DrawInline(rect, boundsColor);

            // Hover overlay
            if (canInteract && hover)
                Display.DrawRect(rect, Color.White * 0.2f);

            // Draw current text
            InputBuilder.Render(pos, true, textColor, !IsFocussed);

            // Draw not-activatable overlay
            if (!canInteract)
                Display.DrawRect(rect, Color.Black * 0.5f);

            // On-click event - set focussed
            if (canInteract && hover && !IsFocussed && Input.IsMouseLeftPressed)
                SetFocussed();

            if (IsFocussed && !hover && Input.IsMouseLeftPressed)
                SetUnFocussed();
        }
    }
}
