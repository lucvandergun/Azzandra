using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Barrel : Instance
    {
        protected bool IsFilled = true;
        protected string Type;
        public override bool IsInteractable() => true;
        public override bool CanBeFlewnOver() => false;

        public override Symbol GetSymbol()
        {
            return IsFilled ? new Symbol('U', Color.Brown)
                : new Symbol('U', Color.Brown.ChangeBrightness(-0.5f));
        }
        public override Color AssetLightness => !IsFilled ? Color.DarkGray : Color.White;
        public override string Name => IsFilled ? ("barrel of " + Type) : "barrel";

        public Barrel(int x, int y, string liquidType) : base(x, y)
        {
            Type = liquidType;
            if (Type == null)
            {
                Type = "";
                IsFilled = false;
            }
                
        }

        public Barrel(int x, int y) : base(x, y) { }


        public override void Interact(Entity entity)
        {
            if (!(entity is Player player))
                return;

            if (!IsFilled)
            {
                player.User.ShowMessage("The barrel seems to be empty.");
                return;
            }

            else
            {
                DrinkData data = player.User.DrinkEffects?.FirstOrDefault(d => d.ID == Type);
                if (data == null)
                {
                    player.User.ShowMessage("You don't know what's in there, but you're not gonna drink it!");
                    return;
                }

                
                if (player.User.Inventory.HasItem(i => i.ID == "tankard"))
                {
                    player.User.Inventory.ReplaceItem(player.User.Inventory.Items.FirstOrDefault(i => i.ID == "tankard"), Item.Create(Type + "_tankard"));
                    player.User.ShowMessage("<spring>You fill your tankard with some " + Type + " from the barrel.");
                    IsFilled = false;
                }
                else
                {
                    data.ApplyEffects(player);

                    player.User.ShowMessage("<spring>You drink the " + Type + " directly from the barrel.");
                    IsFilled = false;
                }
            }
        }


        public override void Load(byte[] bytes, ref int pos)
        {
            IsFilled = BitConverter.ToBoolean(bytes, pos);
            pos += 4;
            Type = GameLoader.ToString(bytes, pos);
            pos += 20;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[24];
            int pos = 0;

            bytes.Insert(pos, BitConverter.GetBytes(IsFilled));
            pos += 4;
            bytes.Insert(pos, GameLoader.GetBytes(Type));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
