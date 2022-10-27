using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class LargeChest : Chest
    {
        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.Orange.ChangeBrightness(-0.3f))
                : new Symbol('¢', Color.Orange);
        }

        public override string Name => "large chest";
        public override string AssetName => IsOpen ? "chest_open_large" : "chest_closed_large";

        public LargeChest(int x, int y, IEnumerable<Item> items) : base(x, y, items) { }
        public LargeChest(int x, int y) : base(x, y) { }
    }
}
