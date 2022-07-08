using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Attack : Affect
    {
        public readonly Style Style;
        public int Acc, Dmg, Def;
        public bool ShowHitMessage { get; set; } = true;
        protected override int GetPropertyAccuracy() => Acc;
        public Attack(Server server, Style style, int speed, int range, float acc, float dmg, List<AttackProperty> properties = null) : base(server, speed, range, properties)
        {
            Style = style;
            Acc = acc.Round();
            Dmg = dmg.Round();
        }


        public override bool RollSuccess(Entity attacker, Instance target)
        {
            if (!(target is Entity cbtarget))
                return false;

            if (attacker == target)
            {
                HitType = HitType.Success;
                Def = 0;
                return true;
            }

            // Get defensive values
            var evd = cbtarget.GetEvd().Round();
            var par = cbtarget.GetPar().Round();
            var blk = cbtarget.GetBlk().Round();

            target.Level.Server.User.ThrowDebug("Rolling acc=" + Acc + " vs evd=" + evd + ", par=" + par + ", blk=" + blk + ":");

            // Roll accuracy against each of the defense values
            if (evd.RollAgainst(Acc * 3))
            {
                Dmg = 0;
                HitType = HitType.Evaded;
                Def = evd;

                return false;
            } 
            else if (Style == Style.Melee && par.RollAgainst(Acc * 3))
            {
                Dmg = 0;
                HitType = HitType.Parried;
                Def = evd + par;

                return false;
            }
            else if (blk.RollAgainst(Acc * 3))
            {
                Dmg = 0;
                HitType = HitType.Blocked;
                Def = evd + par + blk;

                return false;
            }
            else
            {
                HitType = HitType.Success;
                Def = evd + par + blk;

                return true;
            }
        }

        public override void Fail(string msg)
        {
            Acc = 0;
            Dmg = 0;
            base.Fail(msg);
        }

        public override void Apply(Entity attacker, Instance target)
        {
            if (!(target is Entity cbtarget))
                return;

            /*
            // Damage is min 1, max dmg value
            int dmg = Dmg <= 0 ? 0 : Util.Random.Next(Dmg) + 1;
            */

            /*
            // Damage is min 1, max dmg value, with average at 75% of max value
            double alpha = 2.40942; // = ln(2) / ln(1/0.75)
            int dmg = (int)Math.Ceiling(Math.Pow((Util.Random.NextDouble() + 0.001) * Math.Pow(Dmg, alpha), 1 / alpha));

            // Armour is min 0, max arm value & remove roll from dmg
            int arm = Util.Random.Next(target.GetArm(Style) + 1);
            dmg = Math.Max(0, dmg - arm);
            */

            // Damage is proportional compared to the armour value. Armour equal to damage -> halved damage.
            int dmg = Dmg == 0 ? 0
                : (int)Math.Ceiling(Dmg * Dmg / (float)(Dmg + cbtarget.GetArm()));

            // Take a random number between half dmg and full dmg.
            int dmgRand = Util.Random.Next((int)Math.Floor(dmg / 2d) + 1) + (int)Math.Ceiling(dmg / 2d);

            cbtarget.Level.Server.User.ThrowDebug("Rolling dmg=" + Dmg + " vs arm=" + cbtarget.GetArm() + ": --> dmg=" + dmg + ", --> dmg=" + dmgRand);

            Dmg = cbtarget.GetHit(Style, dmgRand);
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
            bool hit = HitType == HitType.Success;

            if (attacker is Player)
            {
                if (target is Player)
                {
                    // Player vs player:
                    if (hit)
                    {
                        return Dmg > 0
                            ? "You hit yourself for <red>" + Dmg + "<r> dmg."
                            : "You hit yourself but it doesn't hurt.";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    // Player vs entity:
                    if (hit)
                    {
                        return Dmg > 0
                            ? "You hit " + target.ToStringAdress() + " for <aqua>" + Dmg.ToString() + "<r> dmg."
                            : "You hit " + target.ToStringAdress() + ".";
                    }
                    else
                    {
                        if (HitType == HitType.Failed)
                            return "You fail to hit " + target.ToStringAdress() + ".";
                        else
                        {
                            var typeName = HitType == HitType.Evaded ? "evades" : HitType == HitType.Parried ? "parries" : "blocks";
                            return target.ToStringAdress().CapFirst() + " " + typeName + " your attack.";
                        }
                    }
                }
            }
            else
            {
                if (target is Player)
                {
                    // Entity vs player:
                    if (hit)
                    {
                        return Dmg > 0
                            ? attacker.ToStringAdress().CapFirst() + " hits you for <red>" + Dmg.ToString() + "<r> dmg."
                            : attacker.ToStringAdress().CapFirst() + " hits you.";
                    }
                    else
                    {
                        if (HitType == HitType.Failed)
                            return attacker.ToStringAdress().CapFirst() + " fails to hit you.";
                        else
                        {
                            var typeName = HitType == HitType.Evaded ? "evade" : HitType == HitType.Parried ? "parry" : "block";
                            return "You " + typeName + " " + attacker.ToStringAdress()+ "'s attack.";
                        }
                    }
                }
                else
                {
                    var targetRef = target == attacker ? "itself" : target.ToStringAdress();
                    // Entity vs entity:
                    if (hit)
                    {
                        return Dmg > 0
                            ? attacker.ToStringAdress().CapFirst() + " hits " + targetRef + " for " + Dmg.ToString() + " dmg."
                            : attacker.ToStringAdress().CapFirst() + " hits " + targetRef + ".";
                    }
                    else
                    {
                        if (HitType == HitType.Failed)
                            return attacker.ToStringAdress().CapFirst() + " fails to hit " + targetRef + ".";
                        else
                        {
                            var typeName = HitType == HitType.Evaded ? "evades" : HitType == HitType.Parried ? "parries" : "blocks";
                            var attackerRef = target == attacker ? "itself" : attacker.ToStringAdress();
                            return target.ToStringAdress().CapFirst() + " " + typeName + " the attack of " + attackerRef + ".";
                        }
                    }
                }
            }
        }
        /*public string CreateMessage(Entity attacker, Instance target)
        {
            bool hit = Type == HitType.Success;

            if (attacker is Player)
            {
                if (target is Player)
                {
                    // Player vs player:
                    if (hit)
                    {
                        return Dmg > 0
                            ? "You hit yourself for <red>" + Dmg + "<r> dmg."
                            : "You hit yourself but it doesn't hurt.";
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                else
                {
                    // Player vs entity:
                    if (hit)
                    {
                        return Dmg > 0
                            ? "You hit " + target.ToStringAdress() + " for <aqua>" + Dmg.ToString() + "<r> dmg."
                            : "You hit " + target.ToStringAdress() + ".";
                    }
                    else
                    {
                        if (Type == HitType.Failed)
                            return "You fail to hit " + target.ToStringAdress() + " with your attack.";
                        else
                        {
                            var typeName = Type == HitType.Evaded ? "evades" : Type == HitType.Parried ? "parries" : "blocks";
                            return "You hit " + target.ToStringAdress() + ", but it " + typeName + " your attack.";
                        }
                    }
                }
            }
            else
            {
                if (target is Player)
                {
                    // Entity vs player:
                    if (hit)
                    {
                        return Dmg > 0
                            ? attacker.ToStringAdress().CapFirst() + " hits you for <red>" + Dmg.ToString() + "<r> dmg."
                            : attacker.ToStringAdress().CapFirst() + " hits you.";
                    }
                    else
                    {
                        if (Type == HitType.Failed)
                            return attacker.ToStringAdress().CapFirst() + " fails to hit you with it's attack.";
                        else
                        {
                            var typeName = Type == HitType.Evaded ? "evade" : Type == HitType.Parried ? "parry" : "block";
                            return attacker.ToStringAdress().CapFirst() + " hits you, but you " + typeName + " its attack.";
                        }
                    }
                }
                else
                {
                    // Entity vs entity:
                    if (hit)
                    {
                        return Dmg > 0
                            ? attacker.ToStringAdress().CapFirst() + " hits " + target.ToStringAdress() + " for " + Dmg.ToString() + " dmg."
                            : attacker.ToStringAdress().CapFirst() + " hits " + target.ToStringAdress() + ".";
                    }
                    else
                    {
                        if (Type == HitType.Failed)
                            return attacker.ToStringAdress().CapFirst() + " fails to hit " + target.ToStringAdress() + " with it's attack.";
                        else
                        {
                            var typeName = Type == HitType.Evaded ? "evades" : Type == HitType.Parried ? "parries" : "blocks";
                            return attacker.ToStringAdress().CapFirst() + " hits " + target.ToStringAdress() + ", but it " + typeName + " its attack.";
                        }
                    }
                }
            }
        }*/


        public override Hit CreateHitDisplay(Entity target)
        {
            string str;
            switch (HitType)
            {
                case HitType.Parried: str = "Parry"; break;
                case HitType.Blocked: str = "Block"; break;
                case HitType.Evaded: str = "Miss"; break;
                case HitType.Failed: str = "Resist"; break;
                default: return null;
            }
            
            return new HitString(target, str);
        }
    }
}
