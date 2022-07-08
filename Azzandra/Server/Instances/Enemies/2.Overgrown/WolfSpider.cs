using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class WolfSpider : Enemy
    {
        public override EntityType EntityType => EntityType.Spider;

        public override int GetW() => 2;
        public override int GetH() => 2;

        public WolfSpider(int x, int y) : base(x, y) { }

        protected override void SetupActionPotentials()
        {
            base.SetupActionPotentials();
            ActionPotentials.Add(
                    new TemplateSpell(2, 2, 12, new SpellEffects.Web())
                    {
                        Requirement = (e) => e.Target.Instance.GetTiles().All(t => e.Level.GetTile(t).Object.ID != BlockID.Cobweb)
                    }
                );
        }

        public override Symbol GetSymbol() => new Symbol('W', new Color(127, 104, 75));
    }
}
