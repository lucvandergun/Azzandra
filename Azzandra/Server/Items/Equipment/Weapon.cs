using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public class Weapon : Equipment
    {
        public override sealed int Slot => 0;
        public override string EquipAction => "wield";

        public int Range = 1;
        public int Speed = 3;

        public virtual Style Style => Style.Melee;
        public bool IsTwoHander = false;
        public bool CanOffHand = false;
        public override bool CanParry => Style == Style.Melee;

        [JsonProperty(PropertyName = "properties")]
        public global::Azzandra.AttackProperty[] AttackProperties = null;

        public override List<string> GetInfo()
        {
            var list = new List<string>();

            list.Add(GetTypeDesc());

            var speedString = global::Azzandra.Equipment.GetSpeedName(Speed) + " (" + Speed + ")"; // TextFormatter.GetValueRatioColor(5 - Speed, 4) + 
            list.Add("Attack speed: " + speedString + ". Range: " + Range + ".");
            list.AddRange(base.GetInfo());

            return list;
        }

        public virtual string GetTypeDesc()
        {
            var str = IsTwoHander ? "Two-handed " : "One-handed ";
            str += Style.ToString().ToLower() + " weapon.";
            if (CanOffHand) str += " Can off-hand.";
            return str;
        }


        public Weapon() : base()
        {

        }


        public override void SetAttributes(Item reference)
        {
            if (reference is Weapon e)
            {
                Range = e.Range;
                Speed = e.Speed;
                IsTwoHander = e.IsTwoHander;
                CanOffHand = e.CanOffHand;

                //Load default properties if none present.
                if (e.AttackProperties != null)
                    Properties.AddRange(e.AttackProperties);
            }

            base.SetAttributes(reference);
        }


        //public override Color StringColor => Color.LightSeaGreen;

        //public override string GetInfo() => Desc + " (" + Attack.GetSignString() + " att)";


        public override List<string> GetOptions()
        {
            var options = new List<string>(3);

            if (Container != null)
            {
                if (Container is Inventory)
                {
                    options.Add(EquipAction);

                    if (CanOffHand)
                        options.Add("off-hand");
                }

            }

            options.AddRange(base.GetOptions());
            return options;
        }

        public override void PerformOption(string option)
        {
            switch (option)
            {
                case "off-hand":
                    Equip(1);
                    return;
                case "cleanse":
                    if (Properties.Count > 0)
                        Properties.RemoveAt(0);
                    return;
            }

            base.PerformOption(option);
        }
    }
}
