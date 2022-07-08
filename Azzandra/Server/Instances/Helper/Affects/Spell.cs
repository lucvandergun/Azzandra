using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Spell : Affect
    {
        public int Spc, Res;
        public SpellEffect SpellEffect;
        protected override int GetPropertyAccuracy() => Spc;

        public Spell(Server server, int speed, int range, float spc, SpellEffect effect, List<AttackProperty> properties = null) : base(server, speed, range, properties)
        {
            SpellEffect = effect;
            Spc = (int)Math.Round(spc, MidpointRounding.AwayFromZero);
        }

        public override bool RollSuccess(Entity attacker, Instance target)
        {
            if (!(target is Entity entity))
            {
                HitType = HitType.Success;
                return true;
            }

            if (attacker == target)
            {
                HitType = HitType.Success;
                Res = 0;
                return true;
            }

            Res = (int)Math.Round(entity.GetRes(), MidpointRounding.AwayFromZero);
            target.Level.Server.User.ThrowDebug("Rolling spc=" + Spc + " vs res=" + Res + ":");

            if (Res.RollAgainst(Spc))
            {
                HitType = HitType.Failed;
                return false;
            }
            else
            {
                HitType = HitType.Success;
                return true;
            }
        }

        public override void Fail(string msg)
        {
            Spc = 0;
            base.Fail(msg);
        }

        public override void Apply(Entity attacker, Instance target)
        {
            SpellEffect.Apply(attacker, target, this);
            //AddMessage(msg);
        }

        public override void AddMainMessage(Entity attacker, Instance target)
        {
            if (!IsFailed()) return;
            
            var actor = (attacker is Player) ? "your" : (attacker.ToStringAdress() + "'s");
            var receiver = (target is Player) ? "You" : target.ToStringAdress().CapFirst();
            var msg = receiver + " resists being affected by " + actor + " spell.";
            AddMessage(msg);
        }

        public override Hit CreateHitDisplay(Entity target)
        {
            return IsFailed() ? new HitString(target, "Resist") : null;
        }
    }
}
