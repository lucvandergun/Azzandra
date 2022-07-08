//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra.ItemType
//{
//    //[JsonConverter(typeof(ItemConverter))]
//    public class General
//    {
//        public string Name;
//        public string Desc;
//        public bool Stack;
//        public string Plural;
//        public string Asset;

//        public string QuantityName(uint qty)
//        {
//            return (qty > 1 && Plural != null) ? Plural : Name;
//        }



//        /// <summary>
//        /// Returns this items' first option on the list. Returns null if none.
//        /// </summary>
//        /// <returns></returns>
//        public string GetDefaultOption()
//        {
//            var options = GetOptions();
//            return options == null ? null : options.Count <= 0 ? null : options[0];
//        }
//        public virtual List<string> GetOptions()
//        {
//            var options = new List<string>(2);

//            if (Container != null)
//                if (Container is Azzandra.Equipment)
//                    options.Add("remove");

//            options.Add("examine");

//            options.Add("drop");
//            if (Amount != 1) options.Add("drop 1");
//            return options;
//        }


//        public void PerformDefaultOption()
//        {
//            PerformOption(GetDefaultOption());
//        }
//        public virtual void PerformOption(string option)
//        {
//            switch (option)
//            {
//                case "examine":
//                    User.Log.Add(Desc);
//                    return;

//                case "drop":
//                    Container.RemoveItem(this);
//                    User.Player.DropItem(this);
//                    User.Log.Add("You drop " + (Amount != 1 ? ToString() : "the " + ToString()) + ".");
//                    return;
//                case "drop 1":
//                    RemoveAmount(1);

//                    var dropItem = (Generic)Activator.CreateInstance(GetType());
//                    dropItem.Amount = 1;

//                    User.Log.Add("You drop the " + dropItem.Name + ".");

//                    return;

//                case "remove":
//                case "unequip":
//                    Unequip();
//                    return;
//            }
//        }


//        // Item Methods:
//        private void Unequip(User user, Item item)
//        {
//            //check whether item is located in equipment
//            if (item.Container != null)
//                if (!(item.Container is Azzandra.Equipment))
//                    user.ThrowError("You don't have the item you're trying to remove equipped.");

//            if (user.Inventory.CanAddItem(this))
//            {
//                item.Container.RemoveItem(this);
//                user.Inventory.AddItem(this);
//                user.Log.Add("You remove " + (item.Quantity != 1 ? ToString() : "the " + ToString()) + ".");
//            }
//            else
//            {
//                user.Log.Add("<rose>You don't have enough space to hold that.");
//            }
//        }


//        public void EmptyItem(User user, Item item)
//        {
//            //check if item is container
//            var container = (IContainer)this;
//            if (container == null)
//                return;

//            var emptyItem = container.Empty;
//            int index = item.FindIndex();
//            item.Container.SetIndex(index, emptyItem);
//            user.Log.Add("You empty the " + Name + ".");
//        }


//        public override string ToString(Item item)
//        {
//            return Quantity != 1
//                ? Name + " (" + Util.StringifyNumber2(Amount) + ")"
//                : Name;
//        }
//        public virtual string ToString2()
//        {
//            return Util.StringifyNumber2(Amount) + " x " + Name;
//        }
//    }
//}
