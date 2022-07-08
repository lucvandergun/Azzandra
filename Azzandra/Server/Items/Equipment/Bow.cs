using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Bow : RangedWeapon
    {
        public override AmmunitionType AmmunitionType => AmmunitionType.Arrow;

        public Bow()
        {

        }
    }
}
