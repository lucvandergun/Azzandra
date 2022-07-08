using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Potion : Drink
    {
        //, IFilledContainer
        //public Item EmptyItem => Item.Create("vial");

        public override string Message => "You quaff the " + Name + ".";

        public override List<string> GetInfo()
        {
            var list = base.GetInfo();

            if (Effects != null)
            {
                //list.Add(Effects.Length > 1 ? "Effects:" : "Effect:");
                Effects.ToList().ForEach(e => list.Add("" + e.GetEffectString() + "<r>"));
            }

            return list;
        }

        public Potion() : base()
        {

        }
    }
}
