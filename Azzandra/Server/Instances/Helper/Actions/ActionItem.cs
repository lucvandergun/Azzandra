using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionItem : EntityAction
    {
        public readonly Item Item;
        public readonly string Option;

        public ActionItem(Entity caller, Item item, string option) : base(caller)
        {
            Item = item;
            Option = option;
        }
        
        protected override bool PerformAction()
        {
            Item?.PerformOption(Option);
            return Item != null;
        }
    }
}
