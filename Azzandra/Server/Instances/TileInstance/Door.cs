
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class Door : Instance
    {
        public virtual bool CanBeOpened() => true;
        
        public bool IsOpen = false;

        public override bool IsInteractable() => true;
        public override bool IsSolid() => !IsOpen;
        public override bool BlocksLight() => !IsOpen;
        public override string AssetName => IsOpen ? "door_open" : "door_closed";


        public override Symbol GetSymbol() => IsOpen ? new Symbol("+", Color.Aqua) : new Symbol("-", Color.Red);
        //‾¬

        public Door(int x, int y) : base(x, y)
        {
            
        }

        /// Saving & Loading:

        public override void Load(byte[] bytes, ref int pos)
        {
            IsOpen = BitConverter.ToBoolean(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(IsOpen));
            pos += 4;

            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override void Interact(Entity entity)
        {
            if (!entity.CanOpenDoors()) return;

            if (IsOpen)
            {
                IsOpen = false;
                var address = entity is Player ? "You close" : (entity.ToStringAdress().CapFirst() + " closes");
                Level.Server.User.ShowMessage("<gray>" + address + " the door.");
                AnimationManager.Play(AssetName);
            }
            else
            {
                IsOpen = true;
                var address = entity is Player ? "You open" : (entity.ToStringAdress().CapFirst() + " opens");
                Level.Server.User.ShowMessage("<gray>" + address + " the door.");
                AnimationManager.Play(AssetName);
            }
        }
    }
}
