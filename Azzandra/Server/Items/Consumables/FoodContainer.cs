using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    //public abstract class FoodContainer : Consumable, IFilledContainer
    //{
    //    public virtual Item EmptyItem => null;

    //    protected override void Consume()
    //    {
    //        Container?.ReplaceItem(this, EmptyItem);
    //        User.Player.Heal(Value);
    //        if (Message != null)
    //            User.ShowMessage(Message, true);
    //    }
    //}
    
    public class PanFood : Food
    {
        //public override Item EmptyItem => Item.Create("pan");
        public override string Message => "You slurp down the " + Name + ".";
    }
}
