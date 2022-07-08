
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class Altar : Instance
    {
        public bool IsUsed { get; protected set; } = false;
        public override bool IsInteractable() => true;


        public override Symbol GetSymbol() => new Symbol("A", Color.White);

        public Altar(int x, int y) : base(x, y)
        {
            
        }

        /// Saving & Loading:
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
                player.User.Log.Add("Its just an altar.");
            }
            else
            {
                player.User.Log.Add("Its just an altar.");
                IsUsed = true;
            }
        }
    }
}
