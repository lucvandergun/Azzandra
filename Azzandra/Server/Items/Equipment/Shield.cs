using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Shield : Equipment
    {
        public override int Slot => 1;
        public override string EquipAction => "wield";

        public override bool CanBlock => true;
        public override bool CanParry => Parry > 0;

        public override List<string> GetInfo()
        {
            var list = new List<string>();
            list.AddRange(base.GetInfo());
            return list;
        }

        public Shield() : base()
        {

        }
    }
}
