using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AlphaWolf : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;


        public AlphaWolf(int x, int y) : base(x, y) { }

        protected override void SetupActionPotentials()
        {
            Spells.Add(new TemplateSpellAcute(new SpellEffects.Howl())
            {
                Requirement = c => c.Children.Any(i => i.Instance is Wolf wolf && wolf.Target?.ID != c.Target?.ID) //  / 2 > c.Children.Count(i => i.Instance is Wolf && i.Instance.TileDistanceTo(c) < 3
            });
        }


        public override Symbol GetSymbol() => new Symbol('W', new Color(184, 184, 184).ChangeBrightness(-0.35f)); //new Color(158, 67, 105)
    }
}
