using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Bomb : Item, ILightable
    {
        public int Size { get; protected set; } = 0;
        public int FuseLength { get; protected set; } = 2;
        public override Color StringColor => Color.Brown;

        public override string GetNameNotNull()
        {
            var name = "bomb";
            name = (Size == 0 ? "small " : Size == 1 ? "" : "large ") + name;
            name += " (" + (FuseLength == 0 ? "no" : FuseLength == 1 ? "short" : FuseLength == 2 ? "medium" : "long") + " fuse)";
            return name;
        }
        
        //public override List<string> GetOptions()
        //{
        //    var options = new List<string>(4) { "light" };
        //    options.AddRange(base.GetOptions());
        //    return options;
        //}

        //public override void PerformOption(string option)
        //{
        //    switch (option)
        //    {
        //        case "light":
        //            Light();
        //            return;
        //    }

        //    base.PerformOption(option);
        //}

        public void Light()
        {
            if (!User.Inventory.HasItem(i => i.ID == "flint_and_steel"))
            {
                User.ShowMessage("You need something to light the fuse with.");
                return;
            }

            User.ShowMessage("<orange>You light fuse of the bomb...");

            Replace(new BombLighted(this));
        }
    }

    public class BombLighted : Item
    {
        public int Size { get; protected set; } = 0;
        public int FuseLength { get; protected set; } = 2;
        public override Color StringColor => Color.OrangeRed;

        public BombLighted(Bomb b)
        {
            if (b != null)
            {
                Size = b.Size;
                FuseLength = b.FuseLength;
                Quantity = b.Quantity;
                ID = "bomb_lighted";
            }
        }

        public BombLighted()
        {

        }

        public override string GetNameNotNull()
        {
            var name = "bomb";
            name = (Size == 0 ? "small " : Size == 1 ? "" : "large ") + name;
            name += " (lighted) [" + FuseLength + "...]";
            return name;
        }
    }
}
