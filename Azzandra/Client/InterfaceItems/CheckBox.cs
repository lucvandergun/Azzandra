using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CheckBox
    {
        public Vector2 Size { get; set; } = new Vector2(24, 24);
        public Action OnClick { get; set; }
        public Color TextColor { get; set; } = Color.White;
        public Color TextColorHover { get; set; } = Color.Aqua;
        public Color TextColorDark { get; set; } = new Color(63, 63, 63);
        public Func<bool> IsSelected { get; set; }
        public Func<bool> CanInteract { get; set; }

        public CheckBox(Action onClick, Func<bool> isSelected)
        {
            OnClick = onClick;
            IsSelected = isSelected;
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
            
            bool hover = Input.MouseHover(surface.Position + pos - Size/2, Size) && canHover;
            var color = isSelected && canInteract ? hover ? TextColorHover : TextColor : TextColorDark;

            // Draw rectangle
            var rect = new Rectangle((pos - Size / 2).ToPoint(), Size.ToPoint());
            Display.DrawRect(rect, new Color(15, 15, 15));
            Display.DrawInline(rect, new Color(63, 63, 63));

            // Hover & selected overlay
            if (canInteract && hover && Input.IsMouseLeftDown)
                Display.DrawRect(rect, Color.White * 0.35f);
            
            // Draw selected checkmark
            if (isSelected || hover && canInteract)
                Display.DrawStringCentered(pos, "x", Assets.Gridfont, color, true);

            // Draw not-activatable overlay
            if (!canInteract)
                Display.DrawRect(rect, Color.Black * 0.5f);

            // On-click event
            if (canInteract && hover && Input.IsMouseLeftReleased)
                OnClick.Invoke();
        }
    }
}
