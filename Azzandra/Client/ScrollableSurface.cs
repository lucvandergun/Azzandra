using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ScrollableSurface : Surface
    {
        public ScrollBar ScrollBar { get; protected set; }

        public Vector2 FullRenderSize { get; set; } = Vector2.One;
        private Vector2 MinRenderOffset = Vector2.Zero;
        private Vector2 MaxRenderOffset => Vector2.Max(Vector2.Zero, FullRenderSize - Size); // Total height - surface height
        private Vector2 _renderOffset = Vector2.Zero;
        public Vector2 RenderOffset
        {
            get => _renderOffset;
            set => _renderOffset = Vector2.Max(MinRenderOffset, Vector2.Min(MaxRenderOffset, value));
        }
        public bool IsSnapped() => RenderOffset == MaxRenderOffset;
        
        public ScrollableSurface(Engine engine) : base(engine)
        {
            var totalHeight = new Func<int>(() => Height);
            var barOffset = new Func<int>(() => (int)Math.Ceiling(RenderOffset.Y / FullRenderSize.Y * Height));
            var barHeight = new Func<int>(() => Math.Min(Height, (int)(Height / FullRenderSize.Y * Height)));
            ScrollBar = new ScrollBar(totalHeight, barOffset, barHeight);
        }

        public void UpdateScrollPosition()
        {
            if (Input.ScrollDirection != 0)
            {
                RenderOffset -= new Vector2(0, Input.ScrollDirection * 20);
            }
        }
    }
}
