using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class FilledTankard : Drink, IFilledContainer
    {
        public Item EmptyItem => Item.Create("tankard");
        public override string Message => "You guzzle down the " + Name + ".";

        public override List<string> GetInfo()
        {
            var list = base.GetInfo();
            var effect = User.DrinkEffects.FirstOrDefault(e => e.ID == ID.Split('_').FirstOrDefault());
            if (effect != null)
            {
                list.Add("Effect: <spring>" + StatusEffectID.GetType(effect.PositiveEffect).Name + "<r>.");
                list.Add("Potential side-effects: " + effect.NegativeEffects.Stringify2(s => "<ltblue>" + StatusEffectID.GetType(s).Name + "<r>") + ".");
            }

            return list;
        }

        protected override void DrinkDrink()
        {
            // Nauseated
            if (User.Player.HasStatusEffect(StatusEffectID.Nausea))
            {
                User.ShowMessage("<acid>You feel too nauseated to even fathom " + ConsumeName + "ing that.");
                return;
            }

            User.ShowMessage(Message);
            var effect = User.DrinkEffects.FirstOrDefault(e => e.ID == ID.Split('_').FirstOrDefault());
            effect?.ApplyEffects(User.Player);

            Replace(Item.Create("tankard"));

            ApplyEffects();
        }


        public override bool OnThrow(Level level, GroundItem grit, Instance inst)
        {
            EmptyOnThrow();
            return true;
        }

        public override void OnThrow(Level level, GroundItem grit, Vector pos)
        {
            EmptyOnThrow();
            return;
        }
    }
}
