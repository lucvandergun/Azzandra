using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Campfire : SingleItemContainer
    {
        public override bool RenderFire => true;
        public override Symbol GetSymbol() => new Symbol('c', Color.DarkOrange);

        public Campfire(int x, int y, Item item = null) : base(x, y)
        {
            IsOpen = true;
            Item = item;
        }

        public Campfire(int x, int y) : base(x, y)
        {
            IsOpen = true;
        }
    }
}
