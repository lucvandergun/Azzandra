using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionRest : EntityAction
    {
        protected override int Delay => 10;
        
        public ActionRest(Entity caller) : base(caller) { }

        public override bool Perform()
        {
            if (!(Caller is Player player)) return false;
            player.User.ShowMessage("You rest for a while.");

            return base.Perform();
        }

        protected override bool PerformAction()
        {
            if (!(Caller is Player player)) return false;

            // Shouldn't happen here, but just in case:
            if (player.Hunger >= player.GetFullHunger())
            {
                player.User.ShowMessage("<rose>You can't rest when you're starving!");
                return false;
            }

            var msg = player.HasStatusEffect(StatusEffectID.Fatigue)
                ? "<lime>You feel recovered, but only slightly as you were very tired."
                : "<lime>You feel fully recovered again!";

            player.Rest();
            player.User.ShowMessage(msg);

            return true;
        }

        public override string ToString()
        {
            return "Resting: " + Timer + "/" + Delay;
        }
    }
}
