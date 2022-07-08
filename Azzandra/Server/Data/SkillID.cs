using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class SkillID
    {
        public static readonly string[] Names = new string[] { "attack", "ranged", "magic", "defense", "evade", "vitality" };
        public static int[] AllIDs => Names.Select((n, i) => i).ToArray();

        public const int // Mind the same ordering as 'Names'!
            Attack = 0,
            Ranged = 1,
            Magic = 2,
            Defense = 3,
            Evade = 4,
            Vitality = 5;

        public static int FromString(string name)
        {
            switch (name)
            {
                case "mgc": name = "magic"; break;
                case "rng": name = "ranged"; break;
                case "evd": name = "evade"; break;
                case "hp": name = "hitpoints"; break;
            }

            for (int i = 0; i < Names.Length; i++)
            {
                if (Names[i].Substring(0, Math.Min(name.Length, Names[i].Length)).Equals(name))
                    return i;
            }

            return -1;
        }

        public static string GetName(int id)
        {
            return id >= 0 && id < Names.Length ? Names[id] : "unid_skill";
        }

        public static string GetShortName(int id)
        {
            switch (GetName(id))
            {
                default: return GetName(id).Substring(0, 3);
                case "magic": return "mgc";
                case "ranged": return "rng";
                case "evade": return "evd";
                case "hitpoints": return "hp";
            }
        }
    }
}
