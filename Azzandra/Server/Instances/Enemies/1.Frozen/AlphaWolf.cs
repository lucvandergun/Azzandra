using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AlphaWolf : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;
        public override float FleeHpThreshold => 0.0f;
        public bool IsLone { get; set; } = false;
        public override string Name => IsLone ? "lone wolf" : "alpha wolf";


        public AlphaWolf(int x, int y) : base(x, y) { }

        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // Active
            IsLone = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[1];
            bytes.Insert(0, BitConverter.GetBytes(IsLone));

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        protected override void SetupActionPotentials()
        {
            Spells.Add(new TemplateSpellAcute(new SpellEffects.Howl())
            {
                Requirement = c => c.Children.Any(i => i.Instance is Wolf wolf && wolf.Target?.ID != c.Target?.ID && wolf.Hp > wolf.FleeHpThreshold * wolf.FullHp) //  / 2 > c.Children.Count(i => i.Instance is Wolf && i.Instance.TileDistanceTo(c) < 3
            });
        }

        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();

            // Select a new wolf to become alpha after a certain amount of time:
            var wolves = Children.Select(c => c.Entity).Where(c => c is Wolf).OrderBy(a => a.Hp).ToList();
            var newAlpha = wolves.LastOrDefault() as Wolf;

            if (newAlpha != null)
            {
                newAlpha.InitiateChangeToAlpha();
                newAlpha.Children.AddRange(wolves.Where(w => w != newAlpha).Select(w => new InstRef(w)));
                wolves.ForEach(w => w.Parent = null);
            }
        }


        public override Symbol GetSymbol() => new Symbol('W', new Color(184, 184, 184).ChangeBrightness(-0.35f)); //new Color(158, 67, 105)
    }
}
