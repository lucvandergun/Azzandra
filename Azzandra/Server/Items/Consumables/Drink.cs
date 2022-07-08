using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Drink : Consumable
    {
        //public override List<string> GetInfo()
        //{
        //    var list = base.GetInfo();
        //    var effect = User.DrinkEffects.FirstOrDefault(e => e.ID == ID.Split('_')[0]);
        //    list.Add("Restores: <lime>" + Value + " hunger<r>.");
        //    return list;
        //}

        public override string ConsumeName => "drink";

        public Drink() : base()
        {

        }

        public override void PerformOption(string option)
        {
            switch (option)
            {
                case "drink":
                    DrinkDrink();
                    return;
            }

            base.PerformOption(option);
        }

        protected virtual void DrinkDrink()
        {
            // Nauseated
            if (User.Player.HasStatusEffect(StatusEffectID.Nausea))
            {
                User.ShowMessage("<acid>You feel too nauseated to even fathom " + ConsumeName + "ing that.");
                return;
            }

            User.ShowMessage(Message);

            RemoveAmount(1);

            ApplyEffects();
        }
    }
}
