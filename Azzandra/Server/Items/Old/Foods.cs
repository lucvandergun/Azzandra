using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    //public class Banana : Food
    //{
    //    public override int Value => 15;
    //    public override string Desc => "A tropical fruit.";
    //}

    //public class Walnut : Food
    //{
    //    public override bool IsStackable => true;
    //    public override int Value => 12;
    //    public override string Message => "<spring>You gobble up the walnut.";
    //    public override string Desc => "A brainy nut.";
    //}

    //public class CandyBar : Food
    //{
    //    public override int Value => 40;
    //    public override string Name => "candy bar";
    //    public override string Message => "You eat the bar of candy. <purple>You notice a slight sugar rush!";
    //    public override string Desc => "A bar made of chocolate.";
    //    public override string AssetID => "candy_bar";

    //    protected override void Eat()
    //    {
    //        base.Eat();
    //        //sugar rush status effect . . . 
    //    }
    //}

    //public class CookedFish : Food
    //{
    //    public override int Value => 25;
    //    public override string Name => "cooked fish";
    //    public override string Message => "You eat the cooked fish.";
    //    public override string Desc => "A nicely cooked fish.";
    //    public override string AssetID => "cooked_fish";

    //    public CookedFish(int quantity = 1)
    //    {
    //        Quantity = quantity;
    //    }
    //}

    //public class WrappedCandyBar : Item
    //{
    //    public override string Name => "wrapped candy bar";
    //    public override string Desc => "You need to unwrap it before you can eat it.";
    //    public override string AssetID => "candy_bar_wrapped";

    //    public override List<string> GetOptions()
    //    {
    //        var options = new List<string>(2) { "unwrap" };
    //        options.AddRange(base.GetOptions());
    //        return options;
    //    }

    //    public override void PerformOption(string option)
    //    {
    //        switch (option)
    //        {
    //            case "unwrap":
    //                Replace(new CandyBar());
    //                User.Log.Add("<pink>You remove the wrapper from the candy bar.");
    //                return;
    //        }

    //        base.PerformOption(option);
    //    }
    //}
}
