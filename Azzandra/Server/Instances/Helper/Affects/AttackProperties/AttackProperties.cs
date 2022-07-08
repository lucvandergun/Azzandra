using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.AttackProperties
{
    public class Fire : AttackProperty
    {
        public Fire(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var success = target.AddStatusEffect(new StatusEffects.Burning(Level, (int)(1.5f + Level*1.5f)));

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<orange>" + specs.Item1 + "set " + specs.Item2 + " on fire.";
            }
            else if (returnFailed)
            {
                return GetFailedMsgSpecs(target) + " cannot be set on fire.";
            }

            return null;
        }
    }

    public class Frost : AttackProperty
    {
        public Frost(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Frozen(1, Level * 5);
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<ltblue>" + specs.Item1 + "frozen " + specs.Item2 + "!";
            }
            else if (returnFailed)
            {
                return GetFailedMsgSpecs(target) + " cannot be " + effect.VariantName + ".";
            }

            return null;
        }
    }

    public class Enchanted : AttackProperty
    {
        public Enchanted(int level = 1) : base(level) { }
    }

    public class Weaken : AttackProperty
    {
        public Weaken(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Weak(Level, 1);
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<purple>" + specs.Item1 + "weakened " + specs.Item2 + "!";
            }
            else if (returnFailed)
            {
                return GetFailedMsgSpecs(target) + " cannot be " + effect.VariantName + ".";
            }

            return null;
        }
    }

    public class Blind : AttackProperty
    {
        public Blind(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Blind(Level, 1);
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<purple>" + specs.Item1 + "blinded " + specs.Item2 + "!";
            }
            else if (returnFailed)
            {
                return GetFailedMsgSpecs(target) + " cannot be " + effect.VariantName + ".";
            }

            return null;
        }
    }

    public class Strength : AttackProperty
    {
        public Strength(int level = 1) : base(level) { }

        public override void EditAffect(Entity attacker, Entity target, Affect affect)
        {
            if (!(affect is Attack attack)) return;

            attack.Dmg = (int)Math.Floor(attack.Dmg * (1f + 0.25f * Level));
        }
    }

    public class Poison : AttackProperty
    {
        public Poison(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Poison(Level, 1);
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<green>" + specs.Item1 + "poisoned " + specs.Item2 + "!";
            }
            else if (returnFailed)
            {
                return GetFailedMsgSpecs(target) + " cannot be " + effect.VariantName + ".";
            }

            return null;
        }
    }

    public class Healing : AttackProperty
    {
        public Healing(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            if (!(affect is Attack attack))
                return null;

            // Heals equal to damage done x% of time: +20% for each level
            if (true)//Util.Random.Next(100) < Level * 20)
            {
                var amt = attacker.Heal(attack.Dmg);
                if (amt > 0)
                {
                    var specs = GetSuccessMsgSpecsHave(attacker, attacker);
                    var color = target is Player ? "<green>" : "<red>";
                    return color + specs.Item1 + "healed " + specs.Item2 + " for " + amt + " hitpoints.";
                }
            }

            return null;
        }
    }

    public class Entangle : AttackProperty
    {
        public Entangle(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Frozen(Level, Level, "entangled");
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);

                // Cast a ray to the target
                attacker.Level.CreateInstance(new Vine(attacker, target));

                return "<cyan>" + specs.Item1 + "entangled " + specs.Item2 + " by a vine!";
            }
            else
            {
                return GetFailedMsgSpecs(target) + " is immune to being entangled.";
            }
        }
    }

    public class Frostbite : AttackProperty
    {
        public Frostbite(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            var effect = new StatusEffects.Frostbite(Level, 1);
            var success = target.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetSuccessMsgSpecsHave(attacker, target);
                return "<ltblue>" + specs.Item1 + "has affected " + specs.Item2 + " with frostbite!";
            }
            else
            {
                return GetFailedMsgSpecs(target) + " is immune to frostbite.";
            }
        }
    }

    public class Knockback : AttackProperty
    {
        public Knockback(int level = 1) : base(level) { }

        public override string Apply(Entity attacker, Entity target, Affect affect, bool returnFailed = false)
        {
            //Vector dist = attacker.DistanceTo(target);
            //Vector push = dist.Sign();

            //var action = new ActionPush(target, push, 3);
            //target.NextAction = action;

            // Calc push vector
            Vector dist = attacker.DistanceTo(target);
            Vector2 angle = dist.ToFloat();
            if (angle != Vector2.Zero) angle.Normalize();
            Vector2 push = new Vector(Level + 1).ToFloat() * angle;

            // Ceil push vector values (floors for negative values):
            Vector pushVector = new Vector(Math.Sign(push.X) * (int)Math.Ceiling(Math.Abs(push.X)), Math.Sign(push.Y) * (int)Math.Ceiling(Math.Abs(push.Y)));
            target.Move(pushVector);

            target.AddStatusEffect(new StatusEffects.Stunned(1, 2));

            var specs = GetSuccessMsgSpecsHave(attacker, target);
            return "<medblue>" + specs.Item1 + "knocked " + specs.Item2 + " back!";
        }
    }
}
