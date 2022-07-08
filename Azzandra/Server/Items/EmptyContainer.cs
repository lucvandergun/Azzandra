using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class EmptyContainer : Item
    {
        /// Extend this class if the item is to be fillable.
        
        public override Color StringColor => Color.BurlyWood;
    }
}
