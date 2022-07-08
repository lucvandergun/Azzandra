using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GelatinousCubeSmall : GelatinousCube
    {
        public override int GetW() => 2;
        public override int GetH() => 2;

        public GelatinousCubeSmall(int x, int y) : base(x, y) { }

        protected override void DropItemsOnDeath()
        {
            foreach (var item in Inventory.Items)
                DropItem(item);
        }
    }
}
