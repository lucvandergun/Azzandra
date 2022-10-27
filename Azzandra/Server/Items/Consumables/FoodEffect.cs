using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class FoodEffect : Property
    {
        public override int GeneralTypeID => FoodID;

        public string ID;
        public int Level = 1;
        public int Time = 1;

        public FoodEffect(string id, int level, int time)
        {
            ID = id;
            Level = level;
            Time = time;
        }

        public FoodEffect() { }

        public bool Apply(Entity entity)
        {
            var player = entity as Player;
            bool isPlayer = player != null;

            var typeID = "Azzandra.StatusEffects." + ID.ToCamelCase();
            var type = Type.GetType(typeID);
            if (type != null)
            {
                var effect = (StatusEffect)Activator.CreateInstance(type, Level, Time, null);
                var s = entity.AddStatusEffect(effect);
                if (isPlayer)
                {
                    if (s)
                    {
                        switch (ID)
                        {
                            case "nausea":
                                player.User.ShowMessage("<acid>You feel nauseated.");
                                break;
                            case "poison":
                                player.User.ShowMessage("<green>You have been poisoned.");
                                break;
                            case "fatigue":
                                player.User.ShowMessage("<medblue>You feel fatigued.");
                                break;
                            default:
                                player.User.ShowMessage("You have been "+effect.ApplyVerb+".");
                                break;
                        }
                    }
                    //else player.User.ShowMessage("You resist being " + effect.ApplyVerb + ".");
                }

                return s;
            }
            
            switch (ID)
            {
                case "heal":
                case "restorehp":
                    var amtHeal = Util.NextUpperHalf(Level);
                    amtHeal = entity.Heal(amtHeal);
                    if (isPlayer)
                    {
                        if (amtHeal > 0)
                            player.User.ShowMessage("<lime>It restores " + amtHeal + " hp.");
                        else
                            player.User.ShowMessage("It doesn't restore any hp, as you were already at full health.");
                    }
                    return true;
                case "boosthp":
                    var amtHpBoost = Util.NextUpperHalf(Level);
                    entity.FullHp += amtHpBoost;
                    if (isPlayer) player.User.ShowMessage("<lime>It has boosted your hitpoints by " + amtHpBoost + " hp.");
                    return true;
                case "restoresp":
                    if (isPlayer)
                    {
                        var amtSp = Util.NextUpperHalf(Level);
                        player.Sp += amtSp;
                        player.User.ShowMessage("<lavender>It restores " + amtSp + " sp.");
                    }
                    return true;
                case "boostsp":
                    if (isPlayer)
                    {
                        var amtSpBoost = Util.NextUpperHalf(Level);
                        player.FullSp += amtSpBoost;
                        player.Sp += amtSpBoost;
                        player.User.ShowMessage("<lavender>It has boosted your spellpoints by " + amtSpBoost + " sp.");
                    }
                    return true;
                case "acid":
                    int dmg = 9;
                    entity.GetHit(Style.Acid, dmg);
                    if (isPlayer)
                        player.User.ShowMessage("<acid>It disintegrates your insides, dealing <red>" + dmg + "<acid> dmg.");
                    return true;
            }

            return false;
        }


        public string GetEffectString()
        {
            string randAmt = (Level + 1) / 2 + "-" + Level;
            switch (ID)
            {
                case "heal":
                case "restorehp":
                    return "Heals: <lime>" + randAmt + " hp<r>.";
                case "boosthp":
                    return "Boosts: <lime>" + randAmt + " hp<r>.";
                case "restoresp":
                    return "Restores: <yellow>" + randAmt + " sp<r>.";
                case "boostsp":
                    return "Boosts: <yellow>" + randAmt + " sp<r>.";
            }

            return "<white>" + ID.Replace('_', ' ').CapFirst() + "<r>";
        }



        public override byte[] ToBytes()
        {
            var bytes = new byte[28];
            int pos = 0;

            // First thing: status effect id
            bytes.Insert(pos, GameSaver.GetBytes(ID));
            pos += 20;

            // Level & Time:
            bytes.Insert(pos, BitConverter.GetBytes(Level));
            pos += 4;
            bytes.Insert(pos, BitConverter.GetBytes(Time));
            pos += 4;

            return bytes;
        }

        public static FoodEffect Load(byte[] bytes, ref int pos)
        {
            string id = GameSaver.ToString(bytes, pos);
            pos += 20;
            int level = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            int time = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            var effect = new FoodEffect(id, level, time);

            return effect;
        }
    }
}
