
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
            if (!(entity is Player player)) return;

            if (IsOpen)
            {
                IsOpen = false;
                player.User.Log.Add("<gray>You close the door.");
                AnimationManager.Play(AssetName);
            }
            else
            {
                IsOpen = true;
                player.User.Log.Add("<gray>You open the door.");
                AnimationManager.Play(AssetName);
            }
        }
    }

    /*
    public class LockedDoor : Door
    {
        public KeyType Key;

        public override Symbol GetSymbol()
        {
            switch (Key)
            {
                default: return new Symbol('?', Color.Orange);
                case KeyType.Brass: return new Symbol('B', Color.Goldenrod);
                case KeyType.Chrome: return new Symbol('C', new Color(220, 220, 220));
                case KeyType.Golden: return new Symbol('G', Color.Gold);
                case KeyType.Iron: return new Symbol('I', new Color(127, 127, 127));
                case KeyType.Nickel: return new Symbol('N', new Color(160, 160, 160));
                case KeyType.Silver: return new Symbol('S', Color.Silver);
            }
        }


        public LockedDoor(int x, int y, rdg.Dir dir, KeyType keyType) : base(x, y, dir)
        {
            Key = keyType;
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;

            var key = player.User.Inventory.CheckItem(Type.GetType("DoA.Item." + Key + "Key"));

            if (key == null)
            {
                player.User.Log.Add("You don't have the correct key to open this door.");
                return;
            }

            if (World.CheckInstanceExists(this))
            {
                Destroy();
                new Door(X, Y, Dir);
                player.User.Log.Add("You unlock the " + Key.ToString().ToLower() + " door.");
                player.User.Inventory.RemoveItem(key);
            }
        }
    }

    public class DirectionalDoor : Door
    {
        public DirectionalDoor(int x, int y, rdg.Dir dir) : base(x, y, dir)
        {

        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;

            if (IsOpen)
            {
                IsOpen = false;
                player.User.Log.Add("<gray>You close the door.");
            }
            else
            {
                //
                if (IsOnRightSide(entity))
                {
                    IsOpen = true;
                    player.User.Log.Add("<gray>You open the door.");
                }
                else
                {
                    player.User.Log.Add("The door seems to be stuck from this direction.");
                }
            }
        }

        public bool IsOnRightSide(Entity entity)
        {
            if (entity.IsCollisionWith(this))
                return false;

            //check entity dir
            var entityDir =
                entity.X > X ? rdg.Dir.East :
                entity.X + entity.GetW() <= X ? rdg.Dir.West :
                entity.Y > Y ? rdg.Dir.North :
                rdg.Dir.South;

            //return true if entity is on right side
            return entityDir == Dir;
        }
    }
    */
}
