using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.StatusEffects
{
    public class Burning : StatusEffectOverTime
    {
        public override string Color => "<orange>";
        protected override int GetApplyDelay() => Math.Max(2 - Level, 1);
        public int GetDamage() => Math.Max(Level - 1, 1); // lvl1: 1, lvl2: 1, onwards: +1

        public Burning(int level, int time, string name = null) : base(level, time, name) { }
        public Burning(byte[] bytes, ref int pos) : base(bytes, ref pos) { }

        protected override void Apply(Entity entity)
        {
            bool alive = entity.Hp > 0;
            entity.GetHit(Style.Fire, GetDamage());
            if (alive && entity.Hp <= 0)
                entity.Level.Server.User.ShowMessage("<orange>" + (entity is Player ? "You are" : entity.ToStringAdress().CapFirst() + " is") + " burnt to a crisp!");
        }
    }

    public class Poison : StatusEffectOverTime
    {
        public override bool IsPermanent => true;
        public override string Color => "<green>";

        protected override int GetApplyDelay() => Math.Max(10 - 3 * (Level - 1), 1);
        public int GetDamage() => 1;

        public Poison(int level, int time, string name = null) : base(level, time, name) { }
        public Poison(byte[] bytes, ref int pos) : base(bytes, ref pos) { }

        protected override void Apply(Entity entity)
        {
            bool alive = entity.Hp > 0;
            entity.GetHit(Style.Poison, GetDamage());
            if (alive && entity.Hp <= 0)
                entity.Level.Server.User.ShowMessage("<green>" + (entity is Player ? "You are" : entity.ToStringAdress().CapFirst() + " is") + " poisoned to death.");
        }
    }

    public class Regeneration : StatusEffectOverTime
    {
        public override string Color => "<lavender>";
        protected override int GetApplyDelay() => Math.Max(5 - Level, 1);
        public int GetHealing() => Math.Max(Level - 3, 1); // below/equals 4: 1, else +1 for every level.

        public Regeneration(int level, int time, string name = null) : base(level, time, name) { }
        public Regeneration(byte[] bytes, ref int pos) : base(bytes, ref pos) { }

        protected override void Apply(Entity entity)
        {
            entity.Heal(GetHealing());
        }
    }

    public class Frozen : StatusEffect
    {
        public override string Color => "<ltblue>";
        public Frozen(int level, int time, string name = null) : base(level, time, name) { }
        public Frozen(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Stunned : StatusEffect
    {
        public override string Color => "<yellow>";
        public Stunned(int level, int time, string name = null) : base(level, time, name) { }
        public Stunned(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Slow : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<medblue>";
        public Slow(int level, int time, string name = null) : base(level, time, name) { }
        public Slow(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Speed : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<yellow>";
        public Speed(int level, int time, string name = null) : base(level, time, name) { }
        public Speed(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Weak : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<purple>";
        public Weak(int level, int time, string name = null) : base(level, time, name) { }
        public Weak(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Blind : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<purple>";
        public Blind(int level, int time, string name = null) : base(level, time, name) { }
        public Blind(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }
    public class Disoriented : StatusEffect
    {
        public override string Color => "<fuchsia>";
        public Disoriented(int level, int time, string name = null) : base(level, time, name) { }
        public Disoriented(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Deflect : StatusEffect
    {
        public override string Color => "<spring>";
        public Deflect(int level, int amount, string name = null) : base(level, amount, name) { }
        public Deflect(byte[] bytes, ref int pos) : base(bytes, ref pos) { }

        public override void Update(Entity entity)
        {
            // This effect does not get decreased in time
        }
    }

    public class Invulnerable : StatusEffect
    {
        public override string Color => "<vred>";
        public Invulnerable(int level, int time, string name = null) : base(level, time, name) { }
        public Invulnerable(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Starving : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<red>";
        public Starving(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Starving(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Fatigue : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<medblue>";
        public Fatigue(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Fatigue(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Nausea : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<acid>";
        public Nausea(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Nausea(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }
    public class Antidote : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<spring>";
        public Antidote(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Antidote(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Antifire : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<dkorange>";
        public Antifire(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Antifire(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Strong : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<lime>";
        public Strong(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Strong(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Accurate : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<azure>";
        public Accurate(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Accurate(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }
    public class Evasive : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<yellow>";
        public Evasive(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Evasive(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Defensive : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<cyan>";
        public Defensive(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Defensive(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }
    public class Sorcerous : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<medblue>";
        public Sorcerous(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Sorcerous(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }

    public class Resistance : StatusEffect
    {
        public override bool IsPermanent => true;
        public override string Color => "<dkred>";
        public Resistance(int level = 1, int time = 0, string name = null) : base(level, time) { }
        public Resistance(byte[] bytes, ref int pos) : base(bytes, ref pos) { }
    }
}
