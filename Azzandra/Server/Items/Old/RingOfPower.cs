//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Azzandra.Items
//{
//    public class RingOfPower : Item
//    {
//        public override string Name => "ring of power";
//        public override string AssetID => "golden_ring";
//        public override string GetInfo() => "A precious ring. ( +100 att)";


//        public override List<string> GetOptions()
//        {
//            var options = new List<string>(2)
//            {
//                "wield"
//            };

//            options.AddRange(base.GetOptions());
//            return options;
//        }

//        public override void PerformOption(string option)
//        {
//            switch (option)
//            {
//                case "wield":
//                    Wield();
//                    return;
//            }

//            base.PerformOption(option);
//        }

//        public void Wield()
//        {
//            Program.Engine.DisplayHandler.CloseInterface();
//            User.Log.Add("<red>You cannot wield it!");
//        }
//    }
//}
