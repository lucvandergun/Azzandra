using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DrinkData
    {
        public static readonly string[] IDs = new string[] { "stout", "mead", "cider", "ale", "bitter", "perry" };
        public static readonly int[] EffectIDs = new int[] { StatusEffectID.Accurate, StatusEffectID.Strong, StatusEffectID.Evasive, StatusEffectID.Defensive, StatusEffectID.Sorcerous, StatusEffectID.Resistance };
        public static readonly int[] NegativeEffectIDs = new int[] { StatusEffectID.Weak, StatusEffectID.Fatigue, StatusEffectID.Nausea }; //, StatusEffectID.Confused

        public string ID { get; private set; }
        public int PositiveEffect = -1;
        public int[] NegativeEffects;

        public StatusEffect CreateStatusEffect(int id, int level) =>
            StatusEffectID.GetType(id) != null
            ? (StatusEffect)Activator.CreateInstance(StatusEffectID.GetType(id), level, 1, null) 
            : null;

        public List<StatusEffect> ApplyEffects(Player player)
        {
            // Drinking directly:
            var effects = new List<StatusEffect>();

            // Positive effect
            var positive = CreateStatusEffect(PositiveEffect, 1);
            player.AddStatusEffect(positive);
            effects.Add(positive);

            // Negative effect
            foreach (var negID in NegativeEffects)
            {
                if (Util.RollAgainst(3, player.User.Stats.GetLevel(SkillID.Vitality)))
                {
                    var negative = CreateStatusEffect(negID, 1);
                    player.AddStatusEffect(negative);
                    effects.Add(negative);
                }
            }

            return effects;
        }


        
        public DrinkData(string id, int effectID, int[] negativeEffectIDs)
        {
            ID = id;
            PositiveEffect = effectID;
            NegativeEffects = negativeEffectIDs;
        }


        public static DrinkData[] AssignDrinkEffects(Random random)
        {
            var ids = IDs.ToList();
            ids.Shuffle(random);
            var list = new List<DrinkData>();
            int positiveIndex = random.Next(EffectIDs.Length);
            int negativeIndex = random.Next(NegativeEffectIDs.Length);

            foreach (var id in ids)
            {
                var positive = EffectIDs[positiveIndex];
                positiveIndex = (positiveIndex + 1) % EffectIDs.Length;

                var negative = NegativeEffectIDs[negativeIndex];
                negativeIndex = (negativeIndex + 1) % NegativeEffectIDs.Length;

                list.Add(new DrinkData(id, positive, new int[] { negative }));
            }

             return list.ToArray();
        }


        public DrinkData(byte[] bytes, ref int pos)
        {
            ID = GameSaver.ToString(bytes, pos);
            pos += 20;
            PositiveEffect = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var amt = BitConverter.ToInt32(bytes, pos);
            NegativeEffects = new int[amt];
            pos += 4;
            for (int i = 0; i < amt; i++)
            {
                NegativeEffects[i] = BitConverter.ToInt32(bytes, pos);
                pos += 4;
            }
        }

        public byte[] ToBytes()
        {
            var bytes = new byte[28];
            bytes.Insert(0, GameSaver.GetBytes(ID));
            bytes.Insert(20, BitConverter.GetBytes(PositiveEffect));;
            bytes.Insert(24, BitConverter.GetBytes(NegativeEffects.Length));
            for (int i = 0; i < NegativeEffects.Length; i++)
            {
                bytes = bytes.Concat(BitConverter.GetBytes(NegativeEffects[i])).ToArray();
            }
            return bytes;
        }
    }
}
