using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Rabnach : Enemy
    {
        public override int GetW() => 3;
        public override int GetH() => 3;
        public override int GetMovementSpeed() => 1;
        public override int GetSightRange() => 20;
        public override int DetectRange => 20;
        public override EntityType EntityType => EntityType.Demon;

        private bool WillPull = false;

        public Rabnach(int x, int y) : base(x, y)
        {
            
        }

        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            WillPull = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(WillPull);

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        protected override void SetupActionPotentials()
        {
            base.SetupActionPotentials();
            
            var fireCage = new TemplateSpell(4, 8, 20, new SpellEffects.FireCage());
            fireCage.Requirement = new Func<Entity, bool>(c => !c.Children.Any(ch => ch.Instance is GroundFire));
            ActionPotentials.Add(fireCage);

            var pull = new TemplateSpell(8, 9, 20, new SpellEffects.MagneticPull());
            pull.Requirement = new Func<Entity, bool>(c => WillPull);
            ActionPotentials.Add(pull);

            var shadowCloud = new TemplateSpell(4, 8, 20, new SpellEffects.ShadowCloud());
            shadowCloud.Requirement = new Func<Entity, bool>(c => !c.Children.Any(ch => ch.Instance is ShadowCloud));
            ActionPotentials.Add(shadowCloud);

            //var summonFiend = new TemplateSpell(4, 8, 20, new SpellEffects.SummonFiend());
            //summonFiend.Requirement = new Func<Entity, bool>(c => !c.Children.Any(ch => ch.Instance is Fiend));
            //ActionPotentials.Add(summonFiend);
        }


        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();

            // Open up victory interface:
            Level.Server.User.Victory();
        }


        public override Symbol GetSymbol() => new Symbol('R', Color.Red);

        public override string ToString() => GetType().Name;
    }
}
