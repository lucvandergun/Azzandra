using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Chest : MultipleItemContainer
    {
        public override Symbol GetSymbol()
        {
            return IsOpen ? HasItems ? new Symbol('c', Color.Orange)
                : new Symbol('c', Color.Orange.ChangeBrightness(-0.3f))
                : new Symbol('¢', Color.Orange);
        }

        public Chest(int x, int y, IEnumerable<Item> items) : base(x, y)
        {
            Inventory.AddItems(items);
        }

        public Chest(int x, int y) : base(x, y) { }
    }
}
