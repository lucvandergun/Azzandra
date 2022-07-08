using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class SpellEffect : ISpellEffect
    {
        public SpellEffect()
        { }

        public abstract void Apply(Entity attacker, Instance target, Affect affect);


        public Tuple<string, string> GetMsgAdressesHave(Instance attacker, Instance target)
        {
            var start =
                    (target is Player) ?
                        (attacker is Player) ? "You have"
                        : attacker.ToStringAdress().CapFirst() + " has"
                    :
                        (attacker is Player) ? "You have"
                        : attacker.ToStringAdress().CapFirst() + " has";
            var end =
                    (target is Player) ?
                        (attacker is Player) ? "yourself"
                        : "you"
                    :
                        (attacker == target) ? "itself"
                        : target.ToStringAdress();

            return Tuple.Create(start, end);
        }
        public Tuple<string, string> GetMsgAdresses(Instance attacker, Instance target)
        {
            var start =
                    (target is Player) ?
                        (attacker is Player) ? "You"
                        : attacker.ToStringAdress().CapFirst()
                    :
                        (attacker is Player) ? "You"
                        : attacker.ToStringAdress().CapFirst();
            var end =
                    (target is Player) ?
                        (attacker is Player) ? "yourself"
                        : "you"
                    :
                        (attacker == target) ? "itself"
                        : target.ToStringAdress();

            return Tuple.Create(start, end);
        }

        public string GetVerb(Instance attacker, string verb) => verb + (!(attacker is Player) ? "s" : "");
        public string GetVerb(Instance attacker, string playerVerb, string enemyVerb) => (attacker is Player ? playerVerb : enemyVerb);

        private string GetFailedMsgSpecs(Instance target)
        {
            return (target is Player) ? "You"
                   : target.ToStringAdress().CapFirst();
        }
    }
}
