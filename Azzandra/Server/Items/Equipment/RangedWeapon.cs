using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public abstract class RangedWeapon : Weapon
    {
        public override Style Style => Style.Ranged;

        public virtual AmmunitionType AmmunitionType => AmmunitionType.Arrow;

        public override List<string> GetInfo()
        {
            var list = base.GetInfo();
            list.Insert(1, "Uses " + AmmunitionType.ToString().ToLower() + "s.");
            return list;
        }

        public RangedWeapon() : base()
        {

        }
    }
}
