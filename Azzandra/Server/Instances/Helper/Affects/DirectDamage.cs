using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DirectDamage : Affect
    {
        public readonly Style Style;
        public int Dmg;
        public readonly bool RollRandom;
        public bool ShowHitMessage { get; set; } = true;
        protected override int GetPropertyAccuracy() => 10000;
        public DirectDamage(Server server, Style style, int dmg, bool rollRandom, List<AttackProperty> properties = null) : base(server, 1, 10, properties)
        {
            Style = style;
            Dmg = dmg;
            RollRandom = rollRandom;
        }


        public override bool RollSuccess(Entity attacker, Instance target)
        {
            HitType = HitType.Success;
            return true;
        }

        public override void Fail(string msg)
        {
            Dmg = 0;
            base.Fail(msg);
        }

        public override void Apply(Entity attacker, Instance target)
        {
            if (!(target is Entity cbtarget))
                return;

            int dmg = Dmg;

            if (Style == Style.Melee || Style == Style.Ranged || Style == Style.Magic)
            {
                // Damage is proportional compared to the armour value. Armour equal to damage -> halved damage.
                dmg = Dmg == 0 ? 0
                    : (int)Math.Ceiling(Dmg * Dmg / (float)(Dmg + cbtarget.GetArm()));
            }
            
            if (RollRandom)
            {
                // Take a random number between half dmg and full dmg.
                dmg = Util.Random.Next((int)Math.Floor(dmg / 2d) + 1) + (int)Math.Ceiling(dmg / 2d);
            }

            Dmg = cbtarget.GetHit(Style, dmg);
        }


        public override void AddMainMessage(Entity attacker, Instance target)
        {
            AddMessage(CreateMessage(attacker, target));
        }

        /// <summary>
        /// Creates a hit message of the attack.
        /// </summary>
        public string CreateMessage(Entity attacker, Instance target)
        {



            if (attacker is Player)
            {
                if (target is Player)
                {
                    // Player vs player:
                    return Dmg > 0
                        ? "You hit yourself for <red>" + Dmg + "<r> dmg."
                        : "You hit yourself but it doesn't hurt.";
                }
                else
                {
                    // Player vs entity:
                    return Dmg > 0
                        ? "You hit " + target.ToStringAdress() + " for <aqua>" + Dmg.ToString() + "<r> dmg."
                        : "You hit " + target.ToStringAdress() + ".";
                }
            }
            else
            {
                if (target is Player)
                {
                    // Entity vs player:
                    return Dmg > 0
                        ? attacker.ToStringAdress().CapFirst() + " hits you for <red>" + Dmg.ToString() + "<r> dmg."
                        : attacker.ToStringAdress().CapFirst() + " hits you.";
                }
                else
                {
                    var targetRef = target == attacker ? "itself" : target.ToStringAdress();
                    // Entity vs entity:
                    return Dmg > 0
                        ? attacker.ToStringAdress().CapFirst() + " hits " + targetRef + " for " + Dmg.ToString() + " dmg."
                        : attacker.ToStringAdress().CapFirst() + " hits " + targetRef + ".";
                }
            }
        }

        public override Hit CreateHitDisplay(Entity target)
        {
            return null;
        }
    }
}
