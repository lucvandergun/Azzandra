using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class EnemyData
    {
        public EntityStats Stats;
        public TemplateAttack[] Attacks;

        public EnemyData()
        {

        }

        public static readonly EnemyData Default = new EnemyData()
        { 
            Stats = new EntityStats(),
            Attacks = new TemplateAttack[] { new TemplateAttack() }
        };
    }
}
