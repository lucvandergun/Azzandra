using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Demon : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Demon;


        public Demon(int x, int y) : base(x, y)
        { }

        //protected override void SetupActionPotentials()
        //{
        //    base.SetupActionPotentials();

        //    ActionPotentials.Add(
        //        new TemplateSpell(3, 4, 7, new SpellEffects.WindBlast())
        //        );
        //}


        public override Symbol GetSymbol() => new Symbol('D', Color.Red);
    }
}
