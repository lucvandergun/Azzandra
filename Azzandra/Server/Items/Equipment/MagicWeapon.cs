using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Items
{
    public abstract class MagicWeapon : Weapon
    {
        public override Style Style => Style.Magic;

        public override Color StringColor =>
            ID.Contains("wind") ? Color.White
            : ID.Contains("ice") ? Color.LightBlue
            : ID.Contains("shadow") ? Color.Purple
            : ID.Contains("fire") ? Color.OrangeRed
            : ID.Contains("poison") ? Color.Green
            : Color.Yellow;

        public MagicWeapon() : base()
        {
            MaxDurability = 60;
        }

        public override void SetAttributes(Item reference)
        {
            if (reference is Staff e)
            {
                MaxDurability = e.MaxDurability;
                if (Durability == -1)
                    Durability = e.MaxDurability;
            }

            base.SetAttributes(reference);
        }

        public void DecreaseDurability(int amt)
        {
            if (MaxDurability <= 0)
                return;

            Durability = Math.Max(0, Durability - amt);
            if (Durability <= 0)
            {
                User.ShowMessage("<medblue>Your " + Name + " ran out of charges and got completely broken in the process.");
                Destroy();
            }
        }
    }
}
