using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class LearnedSpell
    {
        public string ID { get; private set; }
        public int SpcBoost => 2 * Math.Max(0, Level - 1);
        public int Level { get; private set; } = 1;
        public SpellData SpellData { get; private set; }

        public void IncreaseGrasp() => Level++;
        
        public LearnedSpell(string id)
        {
            ID = id;
            Level = 1;
            SpellData = Data.GetSpellData(id);
        }

        public LearnedSpell(byte[] bytes, ref int pos)
        {
            ID = GameSaver.ToString(bytes, pos);
            pos += 20;
            Level = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            SpellData = Data.GetSpellData(ID);
        }

        public byte[] ToBytes()
        {
            var bytes = new byte[24];
            bytes.Insert(0, GameSaver.GetBytes(ID));
            bytes.Insert(20, BitConverter.GetBytes(Level));
            return bytes;
        }
    }
}
