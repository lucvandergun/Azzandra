using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public abstract class Ammunition : Item, IAmmunition
    {
        public int Damage;

        public virtual AmmunitionType AmmunitionType { get; set; } = AmmunitionType.Arrow;
        public List<global::Azzandra.AttackProperty> AttackProperties => null;

        public Ammunition() : base()
        {
            Stack = true;
        }

        public override Color StringColor => ID.Contains("iron") ? Color.Gray : ID.Contains("steel") ? Color.LightGray : ID.Contains("samarite") ? Color.Teal : Color.LightBlue;

        public override List<string> GetInfo()
        {
            var list = base.GetInfo();
            list.Add("When fired:");
            list.Add("Damage: " + GetValueColorCode(Damage) + Damage.GetSignString() + "<r> ");
            return list;
        }

        public override void SetAttributes(Item reference)
        {
            if (reference is Ammunition e)
            {
                Damage = e.Damage;
            }

            base.SetAttributes(reference);
        }
    }
}
