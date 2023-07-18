using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Bookcase : Instance
    {
        public bool HasSpell { get; protected set; } = true;
        public override bool IsInteractable() => true;
        public override bool RenderLightness => !HasSpell;

        public override Symbol GetSymbol()
        {
            return HasSpell ? new Symbol('B', Color.SaddleBrown)
                : new Symbol('B', Color.SaddleBrown.ChangeBrightness(-0.3f));
        }

        public override Color AssetLightness => !HasSpell ? Color.DarkGray : Color.White;

        public Bookcase(int x, int y, bool hasSpell) : base(x, y) { HasSpell = hasSpell; }
        public Bookcase(int x, int y) : base(x, y) { }


        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            HasSpell = BitConverter.ToBoolean(bytes, pos);
            pos += 1;
            base.Load(bytes, ref pos);
        }
        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(HasSpell);
            return bytes.Concat(base.ToBytes()).ToArray();
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;

            if (HasSpell)
            {
                var tomes = new List<string>()
                {
                    "an old tome",
                    "a magic book",
                    "a book about sorcery",
                    "a small book you almost overlooked",
                    "some sort of enchiridion",
                    "manual on esoteric knowledge",
                    "a book about natural powers",
                    "an ancient scroll",
                    "a scroll of divination",
                    "a dusty opus on conjuring"
                };
                
                var spell = Data.GetSpellDataRoll();
                string msg;
                if (player.User.LearnSpell(spell.ID))
                    msg = "<lavender>You find " + tomes.PickRandom() + ". It teaches you the <aqua>" + spell.Name + "<lavender> spell!";
                else
                    msg = "<lavender>You find " + tomes.PickRandom() + ". It teaches you a better grasp on the " + spell.Name + " spell.";
                player.User.Log.Add(msg);
                HasSpell = false;
            }
            else
            {
                player.User.Log.Add("There is nothing of interest in the bookcase.");
            }
        }
    }
}
