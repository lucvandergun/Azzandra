using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public interface IAmmunition
    {
        AmmunitionType AmmunitionType { get; }
        List<global::Azzandra.AttackProperty> AttackProperties { get; }
    }
}
