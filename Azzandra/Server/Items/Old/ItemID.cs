using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class ItemID
    {
        public static string GetTypeID(this Item item)
        {
            return item.GetType().Name.ToLower();
        }
    }
}
