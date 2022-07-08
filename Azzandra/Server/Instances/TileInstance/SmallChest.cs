using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SmallChest : SingleItemContainer
    {
        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.Orange)
                : new Symbol('¢', Color.Orange);
        }

        public SmallChest(int x, int y, Item item = null) : base(x, y)
        {
            Item = item;
        }

        public SmallChest(int x, int y) : base(x, y) { }
    }
}
