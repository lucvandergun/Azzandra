using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Stats
    {
        public User User { get; private set; }
        public const int AMT_OF_SKILLS = 6;

        public Skill[] Skills { get; private set; }

        public Stats(User user)
        {
            User = user;
            InitSkills();
        }

        private void InitSkills()
        {
            Skills = new Skill[AMT_OF_SKILLS];
            for (int i = 0; i < AMT_OF_SKILLS; i++)
            {
                Skills[i] = new Skill(i);
            }
        }

        public int GetLevel(int id)
        {
            if (id >= 0 && id < AMT_OF_SKILLS)
            {
                return Skills[id].Level;
            }

            return 1;
        }

        public void IncreaseLevel(int id, int amount)
        {
            if (id < 0 || id >= AMT_OF_SKILLS)
                return;

            amount = Skills[id].IncreaseLevel(amount);

            // Increase player max hp
            if (id == SkillID.Vitality)
            {
                var incr = User.Player.HpPerLevel * amount;
                User.Player.FullHp += incr;
                User.Player.Hp += incr;
            }
            else if (id == SkillID.Magic)
            {
                var incr = User.Player.SpPerLevel * amount;
                User.Player.FullSp += incr;
                User.Player.Sp += incr;
            }

            ShowLevelUpMessage(Skills[id], amount);
        }

        private void ShowLevelUpMessage(Skill skill, int amount)
        {
            string msg =
                amount == 1 ? "You have increased your " + skill.Name + " to level " + skill.Level + "!"
                : "You have increased " + amount + " " + skill.Name + " levels! It is now level " + skill.Level + "!";

            User.Log.Add("<lime>" + msg);
        }

        public bool SetLevel(int id, int lvl)
        {
            if (id < 0 || id >= AMT_OF_SKILLS)
                return false;

            Skills[id].SetLevel(lvl);

            // Ajust player max hp
            if (User.Player != null)
            {
                if (id == SkillID.Vitality)
                {
                    var realLvl = Skills[id].Level;
                    var amt = User.Player.BaseHp + User.Player.HpPerLevel * realLvl;
                    User.Player.FullHp = amt;
                    User.Player.Hp = amt;
                }
                else if (id == SkillID.Magic)
                {
                    var realLvl = Skills[id].Level;
                    var amt = User.Player.BaseSp + User.Player.SpPerLevel * realLvl;
                    User.Player.FullSp = amt;
                    User.Player.Sp = amt;
                }
            }

            return true;
        }


        // == Saving & Loading == \\
        public byte[] ToBytes()
        {
            var bytes = new byte[0];

            for (int i = 0; i < AMT_OF_SKILLS; i++)
            {
                bytes = bytes.Concat(Skills[i].ToBytes()).ToArray();
            }

            return bytes;
        }

        public void Load(byte[] bytes)
        {
            Skills = new Skill[AMT_OF_SKILLS];
            int pos = 0;

            for (int i = 0; i < AMT_OF_SKILLS; i++)
            {
                Skills[i] = new Skill(bytes, ref pos);
            }
        }
    }
}
