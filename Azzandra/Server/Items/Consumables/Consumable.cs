using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public abstract class Consumable : Item
    {
        public override Color StringColor => Color.Lime;

        [JsonProperty(PropertyName = "effects")]
        public FoodEffect[] Effects = null;

        public virtual string ConsumeName => "eat";
        public virtual string Message => "You " + ConsumeName + " the " + Name + ".";

        public Consumable() : base()
        {

        }

        public override void SetAttributes(Item reference)
        {
            if (reference is Consumable e)
            {
                Effects = e.Effects;
            }

            base.SetAttributes(reference);
        }


        
        public override List<string> GetOptions()
        {
            var options = new List<string>(2) { ConsumeName };
            options.AddRange(base.GetOptions());
            return options;
        }

        /// <summary>
        /// Applies all the foodeffects stated in 'Effects'.
        /// Should be called when the consumable is consumed.
        /// </summary>
        public virtual void ApplyEffects()
        {
            if (Effects != null)
            {
                foreach (var effect in Effects)
                {
                    if (effect.Apply(User.Player))
                    {
                        switch (effect.ID)
                        {
                            case "nausea":
                                User.ShowMessage("<acid>You feel nauseated.");
                                break;
                            case "poison":
                                User.ShowMessage("<green>You have been poisoned.");
                                break;
                        }
                    }
                }
            }
        }
    }
}
