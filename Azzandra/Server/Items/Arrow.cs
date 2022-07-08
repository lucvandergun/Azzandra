using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Arrow : Ammunition
    {
        public override sealed AmmunitionType AmmunitionType => AmmunitionType.Arrow;

        public Arrow() : base()
        {

        }
    }
}
