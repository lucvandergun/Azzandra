
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    
    public class Shrine : Instance
    {
        public bool IsUsed { get; private set; } = false;
        private int[] SkillChoices;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return !IsUsed ? new Symbol("$", Color.SpringGreen)
                : new Symbol("$", Color.SpringGreen.ChangeBrightness(-0.3f));
        }

        public override string AssetName => IsUsed ? "shrine_empty" : "shrine_filled";
        public override Color AssetLightness => IsUsed ? Color.DarkGray : Color.White;

        public Shrine(int x, int y) : base(x, y)
        {
            var skills = SkillID.AllIDs.ToList();
            skills.Shuffle();
            SkillChoices = skills.TakeWhile((s, i) => i < 3).ToArray();
        }


        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            IsUsed = BitConverter.ToBoolean(bytes, pos);
            pos += 1;


            int amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            SkillChoices = new int[amt];
            for (int i = 0; i < amt; i++)
            {
                SkillChoices[i] = BitConverter.ToInt32(bytes, pos);
                pos += 4;
            }

            base.Load(bytes, ref pos);
        }
        public override byte[] ToBytes()
        {
            var bytes = BitConverter.GetBytes(IsUsed);


            int amt = SkillChoices.Length;
            bytes = bytes.Concat(BitConverter.GetBytes(amt)).ToArray();
            foreach (var id in SkillChoices)
            {
                bytes = bytes.Concat(BitConverter.GetBytes(id)).ToArray();
            }


            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;

            if (IsUsed)
            {
                player.User.Log.Add("There is no water left in the shrine.");
            }
            else
            {
                player.User.Server.GameClient.DisplayHandler.Interface = new SkillUpInterface(player.User.Server.GameClient, this, SkillChoices);
            }
        }

        public void UseUp()
        {
            IsUsed = true;
            AnimationManager.Play(AssetName);
        }
    }
}
