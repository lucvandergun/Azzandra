using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct Skill
    {
        public const int MAX_LEVEL = 25;

        public int ID { get; private set; }
        public string Name { get; private set; }
        public string ShortName
        {
            get
            {
                switch (Name)
                {
                    default: return Name.Substring(0, 3);
                    case "magic": return "mgc";
                    case "ranged": return "rng";
                    case "evade": return "evd";
                    case "hitpoints": return "hp";
                }
            }
        }

        public int Level { get; private set; }

        public Skill(int id)
        {
            ID = id;
            Name = SkillID.GetName(id);
            Level = 0;

            SetLevel(1);
        }

        public Skill(int id, int level, int expReq, int expDone)
        {
            ID = id;
            Name = SkillID.GetName(id);
            Level = level;
        }

        public Skill(byte[] bytes, ref int pos)
        {
            ID = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Name = SkillID.GetName(ID);

            Level = BitConverter.ToInt32(bytes, pos);
            pos += 4;
        }



        public int CalcExpReq(int lvl)
        {
            return (int)(75f * Math.Pow(1.15f, lvl - 1));
        }

        public void SetLevel(int level)
        {
            Level = Math.Max(1, Math.Min(MAX_LEVEL, level));
        }

        public int IncreaseLevel(int amount)
        {
            int lvl = Level;
            SetLevel(Level + amount);
            return Level - lvl;
        }


        // == Saving & Loading == \\
        public byte[] ToBytes()
        {
            var bytes = new byte[4 + 4];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(ID));
            pos += 4;
            bytes.Insert(pos, BitConverter.GetBytes(Level));

            return bytes;
        }
    }
}
