using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CarnivorousPlant : Enemy
    {
        public override int GetW() => 2;
        public override int GetH() => 2;
        public override EntityType EntityType => EntityType.Plant;
        public override int DetectRange => 2;
        public override bool CanFlee() => false;

        public CarnivorousPlant(int x, int y) : base(x, y) { }

        protected override EntityAction DetermineAction()
        {
            var target = FindTarget();
            if (target != null)
            {
                var affect = ((TemplateAffect)Attacks[0]).ToAffect(Level.Server);
                if (AttackTimer >= affect.Speed && CanAffect(target, affect))
                    return new ActionAffect(this, target, affect);
            }
            return null;
        }

        protected override bool IsATarget(Entity inst)
        {
            return !inst.IsTypeOf(EntityType.Plant) && inst.IsAttackable();
        }

        public override Symbol GetSymbol() => new Symbol('C', Color.Green);
    }
}
