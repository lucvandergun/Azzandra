using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Container
    {
        public Container()
        {
            
        }
        
        public abstract void RemoveItem(Item item);
        public abstract bool AddItem(Item item);
        public abstract void ReplaceItem(Item item1, Item item2);
        public abstract bool HasItem(Func<Item, bool> predicate);
        public abstract bool CanAddItem(Item item);
        public abstract void Clear();

        public abstract Item GetItemByIndex(int index);
        public abstract int GetIndex(Item item);
    }
}
