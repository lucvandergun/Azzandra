using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionSpellAcute : EntityAction
    {
        public SpellEffectAcute SpellEffect;
        public int Level;

        public ActionSpellAcute(Entity caller, SpellEffectAcute effect, int level = 1) : base(caller)
        {
            SpellEffect = effect;
            Level = level;
        }

        protected override bool PerformAction()
        {
            // Remove player's spellpoints
            if (Caller is Player player)
            {
                var data = Data.GetSpellData(SpellEffect);
                player.Sp -= data.SpellPoints;
            }

            SpellEffect.Apply(Caller, Level);
            return true;
        }
    }
}
