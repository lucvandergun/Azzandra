using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Scroll : Item
    {
        public override Color StringColor => Color.Tan;

        public Scroll() : base()
        {

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
                case "read":
                    Read();
                    return;
            }

            base.PerformOption(option);
        }

        protected virtual void Read()
        {
            User.ShowMessage("You revert the essence back to its idle form.");
            RemoveAmount(1);
        }
    }
}
