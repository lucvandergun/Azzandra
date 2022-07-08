using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    [JsonConverter(typeof(DroptableEntryConverter))]
    public abstract class DroptableEntry
    {
        public int Weight = 1;
        public int Tier = -1;

        public abstract Item[] GetDrop(Random random, int tier);
    }

    public class SingleDrop : DroptableEntry
    {
        public string ItemID;
        public int Quantity = 1;
        public int[] Quantities;
        public int[] QuantityRange;

        public override Item[] GetDrop(Random random, int tier)
        {
            Item item;

            // Random quantity in range
            if (QuantityRange != null && QuantityRange.Length >= 2)
            {
                item = Item.Create(ItemID, random.Next(QuantityRange[0], QuantityRange[1] + 1));
            }

            // One of several quantities
            else if (Quantities != null && Quantities.Length > 0)
            {
                item = Item.Create(ItemID, Quantities[random.Next(Quantities.Length)]);
            }

            // Static quantity
            else
            {
                item = Item.Create(ItemID, Quantity);
            }
            
            return new Item[] { item };
        }
    }

    public class MultipleDrop : DroptableEntry
    {
        public DroptableEntry[] Items;

        public override Item[] GetDrop(Random random, int tier)
        {
            // Concatenates and returns all subitems of this drop.
            var items = new Item[0];
            for (int i = 0; i < Items.Length; i++)
                items = items.Concat(Items[i].GetDrop(random, tier)).ToArray();
            return items;
        }
    }

    public class DroptableDrop : DroptableEntry
    {
        public string Table;

        public override Item[] GetDrop(Random random, int tier)
        {
            return Droptable.RollDrop(Table, random, tier);
        }
    }
}
