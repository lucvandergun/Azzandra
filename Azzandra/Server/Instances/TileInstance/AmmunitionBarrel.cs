using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AmmunitionBarrel : MultipleItemContainer
    {
        public override Symbol GetSymbol()
        {
            return IsOpen ? HasItems ? new Symbol('b', Color.Brown)
                : new Symbol('b', Color.Brown.ChangeBrightness(-0.3f))
                : new Symbol('b', Color.Brown);
        }

        public override string AssetName => HasItems ? "ammunition_barrel_open" : "ammunition_barrel_closed";

        public AmmunitionBarrel(int x, int y, IEnumerable<Item> items) : base(x, y)
        {
            Inventory.AddItems(items);
        }

        public AmmunitionBarrel(int x, int y) : base(x, y) { }
    }
}
