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
            var typeID = "Azzandra.StatusEffects." + ID.ToCamelCase();
            var type = Type.GetType(typeID);
            if (type != null)
            {
                var effect = (StatusEffect)Activator.CreateInstance(type, Level, Time, null);
                var s = entity.AddStatusEffect(effect);

                return s;
            }

            var player = entity as Player;
            
            switch (ID)
            {
                case "heal":
                case "restorehp":
                    var amtHeal = Util.NextUpperHalf(Level);
                    entity.Heal(amtHeal);
                    return true;
                case "boosthp":
                    var amtHpBoost = Util.NextUpperHalf(Level);
                    entity.FullHp += amtHpBoost;
                    return true;
                case "restoresp":
                    if (player != null)
                    {
                        var amtSp = Util.NextUpperHalf(Level);
                        player.Sp += amtSp;
                        player.Heal(amtSp);
                    }
                    return true;
                case "boostsp":
                    if (player != null)
                    {
                        var amtSpBoost = Util.NextUpperHalf(Level);
                        player.FullSp += amtSpBoost;
                        player.Sp += amtSpBoost;
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
