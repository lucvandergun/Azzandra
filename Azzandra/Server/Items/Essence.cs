using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Essence : Item
    {
        public override Color StringColor => Color.SpringGreen;

        public Essence() : base()
        {
            Stack = true;
        }


        //public override List<string> GetOptions()
        //{
        //    var options = new List<string>(3) { "release" };
        //    options.AddRange(base.GetOptions());
        //    return options;
        //}

        public override void PerformOption(string option)
        {
            switch (option)
            {
                case "release":
                    Release();
                    return;
            }

            base.PerformOption(option);
        }

        protected virtual void Release()
        {
            var item = Item.Create("idle_essence");
            if (User.Inventory.CanAddItem(item))
            {
                RemoveAmount(1);
                User.Inventory.AddItem(item);
                User.ShowMessage("You revert the essence back to its idle form.");
            }
            else
            {
                User.ShowMessage("<rose>You dont have enough inventory space to do that.");
            }
        }
    }
}
