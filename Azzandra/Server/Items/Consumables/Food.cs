using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Food : Consumable
    {
        public override List<string> GetInfo()
        {
            var list = base.GetInfo();
            list.Add("Restores: <lime>" + Value + " hunger<r>.");
            return list;
        }

        public int Value = 1;

        public override string ConsumeName => "eat";

        public Food() : base()
        {

        }

        public override void SetAttributes(Item reference)
        {
            if (reference is Food e)
            {
                Value = e.Value;
            }

            base.SetAttributes(reference);
        }

        public override void PerformOption(string option)
        {
            switch (option)
            {
                case "eat":
                    Eat();
                    return;
            }

            base.PerformOption(option);
        }

        protected virtual void Eat()
        {
            // Nauseated
            if (User.Player.HasStatusEffect(StatusEffectID.Nausea))
            {
                User.ShowMessage("<acid>You feel too nauseated to even fathom " + ConsumeName + "ing that.");
                return;
            }

            // Too full
            if (User.Player.Hunger <= 0)
            {
                User.ShowMessage("You feel too full already to eat that.");
                return;
            }
            
            RemoveAmount(1);
            User.Player.Hunger -= Value;
            var msg = User.Player.Hunger <= 0
                ? " You are very full now."
                : " You are less hungry now.";
            User.ShowMessage(Message + msg);


            ApplyEffects();


            //var amt = User.Player.Heal(Value);
            //if (Message != null)
            //{
            //    var amtMsg = " It heals <lime>" + amt + " hp<r>.";
            //    User.ShowMessage(Message + amtMsg, true);
            //}  
        }
    }
}
