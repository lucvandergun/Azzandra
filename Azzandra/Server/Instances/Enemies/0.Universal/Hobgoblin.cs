using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Hobgoblin : Enemy
    {
        public override EntityType EntityType => EntityType.Goblin;
        
        Hobgoblin(int x, int y) : base(x, y) { }

        public override Symbol GetSymbol() => new Symbol('h', Color.Peru);

        protected override bool IsATarget(Entity inst)
        {
            if (inst is Goblin)
                return true;

            return base.IsATarget(inst);
        }
    }
}
