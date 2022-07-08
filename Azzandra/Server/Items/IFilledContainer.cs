using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public interface IFilledContainer
    {
        /// Implement this interface if the item is emptyable.
        ///     It's empty-logic is handled in the general item class.

        Item EmptyItem { get; }
    }
}
