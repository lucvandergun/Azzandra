using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Button : InterfaceItem
    {
        public Vector2 Size { get; set; }
        public string DefaultText { get; set; }
        public Action<Rectangle> Format { get; set; } = ButtonFormat.Menu; // Action to draw the button bg.
        public Action OnClick { get; set; }
        public Func<string> Text { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public Color TextColorHover { get; set; } = Color.Aqua;
        public Func<bool> IsSelected { get; set; }
        public Func<bool> CanInteract { get; set; }

        private string GetCurrentButtonText()
        {
            return Text != null ? Text() : DefaultText;
        }

        public override void OnEnterKey()
        {
            OnClick();
            OnTabKey();
        }

        public Button(Vector2 size, string defaultText, Action<Rectangle> buttonFormat = null)
        {
            Size = size;
            DefaultText = defaultText;
            if (buttonFormat != null)
                Format = buttonFormat;
        }

        /// <summary>
        /// Renders the button at the specified position in the surface.
        /// Invokes the OnClick method when leftclicked.
        /// </summary>
        /// <param name="surface">The surface the button is drawn in.</param>
        /// <param name="pos">The relative position inside the surface to draw the button centered.</param>
        /// <param name="gd">The GraphicsDevice</param>
        /// <param name="sb">The SpriteBatch</param>
        /// <param name="canHover">Whether the button can be hovered.</param>
        public void Render(Surface surface, Vector2 pos, GraphicsDevice gd, SpriteBatch sb, bool canHover)
        {
            bool canInteract = CanInteract == null || CanInteract();
            bool isSelected = IsSelected != null && IsSelected();

            if (canInteract && IsFocussed) UpdateKeyInput();

            bool hover = Input.MouseHover(surface.Position + pos - Size/2, Size) && canHover;
            var color = canInteract && hover ? TextColorHover : TextColor;

            // Draw rectangle
            var rect = new Rectangle((pos - Size / 2).ToPoint(), Size.ToPoint());
            Format?.Invoke(rect);

            if (IsFocussed)
                Display.DrawInline(rect, Color.White);

            // Hover & selected overlay
            if (canInteract && hover && Input.IsMouseLeftDown)
                Display.DrawRect(rect, Color.White * 0.35f);
            else if (isSelected)
                Display.DrawRect(rect, Color.White * 0.25f);

            // Draw button text
            var format = new TextFormat(color, Assets.Gridfont, Alignment.Centered, true);
            TextFormatter.DrawMultiLineString(pos, GetCurrentButtonText().Split('\n'), format);

            // Draw not-activatable overlay
            if (!canInteract)
                Display.DrawRect(rect, Color.Black * 0.5f);

            // On-click event
            if (canInteract && hover && Input.IsMouseLeftReleased)
                OnClick.Invoke();
        }

        //protected virtual void DrawForm(Rectangle rect)
        //{
        //    Display.DrawRect(rect, new Color(31, 31, 31));
        //    Display.DrawInline(rect, new Color(63, 63, 63));
        //}
    }
}
