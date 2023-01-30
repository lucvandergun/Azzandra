using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class StatusEffectID
    {
        public const int
            // Negative
            Burning = 0,
            Frozen = 1,
            Poison = 2,
            Slow = 3,
            Weak = 4,
            Stunned = 5,
            Blind = 6,
            Disoriented = 7,
            Bleeding = 8,
            Frostbite = 9,
            Starving = 10,
            Fatigue = 11,
            Confused = 12,
            Nausea = 13,

            // Positive
            Speed = 20,
            Regeneration = 21,

            Strong = 22,
            Accurate = 23,
            Evasive = 24,
            Defensive = 25,
            Sorcerous = 26,
            Resistance = 27,

            Protection = 28,
            Deflect = 29,
            Invulnerable = 30,
            Antidote = 31,
            Antifire = 32;

        private static readonly Dictionary<int, Type> StatusEffectIDs = new Dictionary<int, Type>()
        {
            { Burning, typeof(StatusEffects.Burning) },
            { Frozen, typeof(StatusEffects.Frozen) },
            { Poison, typeof(StatusEffects.Poison) },
            { Stunned, typeof(StatusEffects.Stunned) },
            { Weak, typeof(StatusEffects.Weak) },
            { Blind, typeof(StatusEffects.Blind) },
            { Regeneration, typeof(StatusEffects.Regeneration) },
            { Slow, typeof(StatusEffects.Slow) },
            { Speed, typeof(StatusEffects.Speed) },
            { Disoriented, typeof(StatusEffects.Disoriented) },
            { Deflect, typeof(StatusEffects.Deflect) },
            { Starving, typeof(StatusEffects.Starving) },
            { Fatigue, typeof(StatusEffects.Fatigue) },
            { Nausea, typeof(StatusEffects.Nausea) },
            { Invulnerable, typeof(StatusEffects.Invulnerable) },
            { Antidote, typeof(StatusEffects.Antidote) },
            { Antifire, typeof(StatusEffects.Antifire) },
            { Accurate, typeof(StatusEffects.Accurate) },
            { Strong, typeof(StatusEffects.Strong) },
            { Evasive, typeof(StatusEffects.Evasive) },
            { Defensive, typeof(StatusEffects.Defensive) },
            { Sorcerous, typeof(StatusEffects.Sorcerous) },
            { Resistance, typeof(StatusEffects.Resistance) },
        };

        public static int GetID(this StatusEffect effect)
        {
            if (effect == null) return -1;
            
            if (StatusEffectIDs.Any(x => x.Value == effect.GetType()))
            {
                return StatusEffectIDs.First(x => x.Value == effect.GetType()).Key;
            }
            
            return -1;
        }

        public static Type GetType(int id)
        {
            if (StatusEffectIDs.TryGetValue(id, out var t))
                return t;
            else
                return null;
        }

        //public static StatusEffect LoadStatusEffect(byte[] bytes, ref int pos)
        //{
        //    int id = BitConverter.ToInt32(bytes, pos += 4);

        //    var type = StatusEffectID.GetType(id);
        //    if (type != null && typeof(StatusEffect).IsAssignableFrom(type))
        //    {
        //        return (StatusEffect)Activator.CreateInstance(type, bytes, pos);
        //    }

        //    return null;
        //}
    }
}
