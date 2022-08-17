using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class FoodEffect
    {
        public string ID;
        public int Level = 1;
        public int Time = 1;

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
                    entity.Heal(amtHeal);
                    if (isPlayer) player.User.ShowMessage("<lime>It restores "+amtHeal+" hp.");
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
                        player.User.ShowMessage("<lavender>It restores " + amtSp + "sp.");
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
    }
}
