using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{

    [JsonConverter(typeof(AttackPropertyConverter))]
    public abstract class AttackProperty : Property
    {
        public override int GeneralTypeID => AttackID;

        //[JsonConverter(typeof(AttackPropertyIDConverter))]
        //public readonly int ID;
        //[DefaultValue(1)]
        //[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public readonly int Level;

        public AttackProperty(int level = 1)
        {
            Level = level;
        }

        public AttackProperty()
        {
            Level = 1;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AttackProperty p))
                return false;
            return GetType() == p.GetType() && Level == p.Level;
        }


        public override byte[] ToBytes()
        {
            var bytes = new byte[8];
            int pos = 0;

            // First thing: status effect id
            bytes.Insert(pos, BitConverter.GetBytes(this.GetID()));
            pos += 4;

            // Level:
            bytes.Insert(pos, BitConverter.GetBytes(Level));
            pos += 4;

            return bytes;
        }

        public static AttackProperty Load(byte[] bytes, ref int pos)
        {
            // Get id & retrieve type from it.
            int id = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var type = AttackPropertyID.GetType(id);

            if (!typeof(AttackProperty).IsAssignableFrom(type))
                return null;

            int level = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            var prop = (AttackProperty)Activator.CreateInstance(type, level);

            return prop;
        }


        public override string ToString()
        {
            var levelStr = Level > 1 ? " " + Level.ToRoman() : "";
            return GetType().Name.CapFirst() + levelStr;
        }

        public string ToDebugString()
        {
            return "id: " + GetType().Name.CapFirst() + " " + Level;
        }


        /// <summary>
        /// The initial apply chance (0 to 1), if this roll succeeds, the affects accuracy might be rolled against the target's resistance stat if enabled.
        /// </summary>
        public virtual float ApplyChance => 0.2f.LerpTo(0.5f, (Level - 1) / 3f);

        /// <summary>
        /// Whether to roll a secondary apply chance: the affects accuracy against the target's resistance value.
        /// </summary>
        public virtual bool RollAccuracy => false;



        public virtual void EditAffect(Entity attacker, Entity target, Affect affect)
        {

        }

        public virtual string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            return null;
        }


        protected Tuple<string, string> GetSuccessMsgSpecsHave(Entity attacker, Entity target)
        {
            var start =
                    (target is Player) ?
                        (attacker is Player) ? "You have "
                        : "" + attacker.ToStringAdress().CapFirst() + " has "
                    :
                        (attacker is Player) ? "You have "
                        : "" + attacker.ToStringAdress().CapFirst() + " has ";
            var end =
                    (target is Player) ?
                        (attacker is Player) ? "yourself"
                        : "you"
                    :
                        (attacker == target) ? "itself"
                        : target.ToStringAdress();

            return Tuple.Create(start, end);
        }

        protected string GetFailedMsgSpecs(Entity target)
        {
            return (target is Player) ? "You "
                   : target.ToStringAdress().CapFirst();
        }
    }
}
