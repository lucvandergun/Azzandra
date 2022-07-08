using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ScrollBar
    {
        public static readonly int Width = 4;
        public Func<int> GetValue { get; set; }
        public Action<int> SetValue { get; set; }

        private Func<int> Height, BarYOffset, BarHeight;
        public Vector2 Size => new Vector2(Width, Height.Invoke());
        private Vector2 BarOffset => new Vector2(0, BarYOffset.Invoke());
        private Vector2 BarSize => new Vector2(Size.X, BarHeight.Invoke());


        public Func<bool> IsSelected { get; set; }
        public Func<bool> CanInteract { get; set; }

        public ScrollBar(Func<int> height, Func<int> barOffset, Func<int> barHeight)
        {
            Height = height;
            BarYOffset = barOffset;
            BarHeight = barHeight;

        }

        /// <summary>
        /// Renders the button at the specified position in the surface.
        /// Allows repositioning of the scrollbar offset and therewith scrolling the accompanying value.
        /// </summary>
        /// <param name="surface">The surface the button is drawn in.</param>
        /// <param name="relativePos">The relative position inside the surface to draw the scrollbar centered.</param>
        /// <param name="canHover">Whether the button can be hovered.</param>
        public void Render(Vector2 regionOffset, Vector2 relativePos, bool canHover)
        {
            // Don't draw if there is nothing to be scrolled.
            if (BarSize == Size || BarHeight?.Invoke() <= 0)
                return;
            
            bool canInteract = CanInteract == null || CanInteract();
            bool isSelected = IsSelected != null && IsSelected();

            var startPoint = relativePos - Size / 2;

            bool hoverFull = Input.MouseHover(regionOffset + startPoint, Size) && canHover;
            bool hoverBar = Input.MouseHover(regionOffset + startPoint + BarOffset, BarSize) && canHover;

            // Draw outline & bar section
            Display.DrawInline(Display.MakeRectangle(startPoint, Size), new Color(31, 31, 31));
            var bar = Display.MakeRectangle(startPoint + BarOffset, BarSize);
            Display.DrawRect(bar, new Color(127, 127, 127));
            Display.DrawInline(bar, new Color(96, 96, 96));

            // Hover & selected overlay
            if (canInteract && hoverBar && Input.IsMouseLeftDown)
                Display.DrawRect(bar, Color.White * 0.35f);
            else if (isSelected)
                Display.DrawRect(bar, Color.White * 0.25f);

            // Draw not-activatable overlay
            if (!canInteract)
                Display.DrawRect(bar, Color.Black * 0.5f);

            // On-click event
            if (canInteract && Input.IsMouseLeftPressed)
            {
                if (hoverBar)
                {

                }
                else
                {

                }
            }
        }
    }
}
