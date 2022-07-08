//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra.Items
//{
//    public abstract class Itemx
//    {
//        public static User User;

//        public Container Container;

//        public int Amount
//        { 
//            get => _amount;
//            set
//            {
//                _amount = Math.Max(0, value);
//                if (_amount <= 0)
//                    Destroy();
//            }
//        }

//        private int _amount;

//        public virtual bool IsStackable => false;
//        public virtual string Name => GetType().Name.ToLower();
//        public virtual string AssetID => null;
//        public virtual string SingularAssetID => AssetID;

//        public virtual string Desc => "Unknown item.";
//        public virtual string GetInfo() => Desc;
//        public virtual Color StringColor => Color.White;



//        public Itemx(int amount = 1)
//        {
//            Amount = amount;
//        }

//        public void Destroy()
//        {
//            //remove slot from container
//            Container.RemoveItem(this);
//        }
//        public void RemoveAmount(int amount)
//        {
//            if (amount >= Amount)
//            {
//                Destroy();
//            }
//            else
//            {
//                Amount -= amount;
//            }
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

//                    var dropItem = (Item)Activator.CreateInstance(GetType());
//                    dropItem.Amount = 1;

//                    User.Log.Add("You drop the " + dropItem.Name + ".");
                    
//                    return;

//                case "remove":
//                case "unequip":
//                    Unequip();
//                    return;
//            }
//        }

//        private void Unequip()
//        {
//            //check whether item is located in equipment
//            if (Container != null)
//                if (!(Container is Azzandra.Equipment))
//                    User.ThrowError("You don't have the item you're trying to remove equipped.");

//            if (User.Inventory.CanAddItem(this))
//            {
//                Container.RemoveItem(this);
//                User.Inventory.AddItem(this);
//                User.Log.Add("You remove " + (Amount != 1 ? ToString() : "the " + ToString()) + ".");
//            }
//            else
//            {
//                User.Log.Add("<rose>You don't have enough space to hold that.");
//            }
//        }


//        public void Replace(Item item)
//        {
//            if (Container == null)
//            {
//                User.ThrowDebug("The item to replace doesn't have a container");
//                return;
//            }

//            Container.ReplaceItem(this, item);
//        }


//        public override string ToString()
//        {
//            return Amount != 1
//                ? Name + " (" + Util.StringifyNumber2(Amount) + ")"
//                : Name;
//        }
//        public virtual string ToString2()
//        {
//            return Util.StringifyNumber2(Amount) + " x " + Name;
//        }
//    }
//}
