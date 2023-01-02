using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Potion : Drink
    {
        //, IFilledContainer
        //public Item EmptyItem => Item.Create("vial");

        public override string Message => "You quaff the " + Name + ".";

        public override List<string> GetInfo()
        {
            var list = base.GetInfo();

            if (Effects != null)
            {
                //list.Add(Effects.Length > 1 ? "Effects:" : "Effect:");
                Effects.ToList().ForEach(e => list.Add("" + e.GetEffectString() + "<r>"));
            }

            return list;
        }

        public Potion() : base()
        {

        }


        public override bool OnThrowOnInstance(Level level, GroundItem grit, Instance inst)
        {
            base.OnThrowOnInstance(level, grit, inst);

            level.CreateInstance(new PotionCloud(grit.X, grit.Y, GetFoodEffects().ToArray()));
            grit.DestroyNextTurn();
            var name = inst == User.Player ? "you" : inst.ToStringAdress();
            User.Log.Add("<gray>The glass vial shattered as it hit " + name + ", freeing the liquid inside.");

            return true;
        }

        public override void OnThrowOnTile(Level level, GroundItem grit, Vector pos)
        {
            level.CreateInstance(new PotionCloud(grit.X, grit.Y, GetFoodEffects().ToArray()));
            grit.DestroyNextTurn();
            User.Log.Add("<gray>The glass vial shattered as it hit the floor, freeing the liquid inside.");
            return;
        }
    }
}
