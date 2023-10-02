using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Wolf : Enemy
    {
        public override EntityType EntityType => EntityType.Beast;

        public override bool ReturnHome() => base.ReturnHome() && !(Parent?.Instance is AlphaWolf aw && IsInRange(aw, 3));
        public int AlphaTimer = -1;

        protected override bool IsAgressive() => Parent != null;
        public override float FleeHpThreshold => Parent == null ? 2.0f : base.FleeHpThreshold;


        public Wolf(int x, int y) : base(x, y) { }


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // AlphaTimer:
            AlphaTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            bytes.Insert(0, BitConverter.GetBytes(AlphaTimer));

            return bytes.Concat(base.ToBytes()).ToArray();
        }


        public override void TurnStart()
        {
            base.TurnStart();

            // Change to Alpha Wolf if timer indicates this:
            if (AlphaTimer > 0) AlphaTimer--;
            else if (AlphaTimer == 0)
            {
                AlphaTimer = 0;
                ChangeToAlpha();
            }
        }

        protected override EntityAction DetermineAction()
        {
            // Flee if no parent, but has a target.
            if (!IsFleeing && !(Parent?.Instance is AlphaWolf) && Target != null)
            {
                SetFleeing();
            }
            return base.DetermineAction();
        }

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            // Flee if no parent and get attacked.
            if (!(Parent?.Instance is AlphaWolf))
            {
                Target = new InstRef(attacker);
                SetFleeing();
            }
            return base.GetAffected(attacker, affect);
        }

        public override Symbol GetSymbol() => new Symbol('w', new Color(184, 184, 184));

        protected override bool IsTargetStillValid()
        {
            return Parent?.Instance is AlphaWolf aw && aw.Target?.Instance == Target?.Instance || base.IsTargetStillValid();
        }

        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();

            // Change Parent to "lone wolf" if this was its last wolf child.
            if (Parent?.Instance is AlphaWolf aw)
            {
                if (aw.Children.Count(c => c.Instance is Wolf) == 1)
                {
                    aw.IsLone = true;
                }
            }

            // If the to-be-alpha wolf is killed before he became an alpha wolf, determine a new alpha wolf:
            if (AlphaTimer > 0)
            {
                // Select a new wolf to become alpha after a certain amount of time:
                var wolves = Children.Select(c => c.Entity).Where(c => c is Wolf).OrderBy(a => a.Hp).ToList();
                var newAlpha = wolves.LastOrDefault() as Wolf;

                if (newAlpha != null)
                {
                    newAlpha.InitiateChangeToAlpha();
                    newAlpha.Children.AddRange(wolves.Where(w => w != newAlpha).Select(w => new InstRef(w)));
                }
            }
        }

        public void InitiateChangeToAlpha()
        {
            AlphaTimer = 14;
        }

        protected void ChangeToAlpha()
        {
            // Replace this instance by a newly created AlphaWolf instance - copy some attributes over:
            var aw = new AlphaWolf(X, Y);
            Level.ReplaceInstance(this, aw);

            aw.Target = Target;
            aw.Hp = Hp;
            aw.StatusEffects = StatusEffects;
            aw.TimeSinceLastTurn = TimeSinceLastTurn;
            aw.Children.AddRange(Children);

            // Notify all of the wolf-children of this update:
            Children.Where(c => c.Entity is Wolf).ToList().ForEach(w => w.Instance.Parent = new InstRef(aw));

            if (Children.Where(c => c.Entity is Wolf).Count() > 0)
            {
                Level.Server.User.ShowMessage("<slate>One of the wolves became the new leader of the pack.");
            }
            else
            {
                aw.IsLone = true;
            }
                
        }
    }
}
