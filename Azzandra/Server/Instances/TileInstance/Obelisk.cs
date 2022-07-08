
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class Obelisk : Instance
    {
        public bool IsUsed { get; protected set; } = false;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return !IsUsed ? new Symbol("O", Color.SkyBlue)
                : new Symbol("O", Color.SkyBlue.ChangeBrightness(-0.3f));
        }

        public Obelisk(int x, int y) : base(x, y)
        {
            
        }

        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            IsUsed = BitConverter.ToBoolean(bytes, pos);
            pos += 1;
            base.Load(bytes, ref pos);
        }
        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(IsUsed);
            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;

            if (IsUsed)
            {
                player.User.Log.Add("The obelisk is out of power.");
            }
            else if (player.Sp < player.GetFullSp())
            {
                player.Sp = player.GetFullSp();
                player.User.Log.Add("<yellow>You feel the power of the obelisk flow through you! It recharges all of your spell points.");
                IsUsed = true;
            }
            else
            {
                player.User.Log.Add("You feel a presence of power inside the obelisk, but there is nothing it can do for you now.");
            }
        }
    }
}
