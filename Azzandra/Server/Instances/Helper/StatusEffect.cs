using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class StatusEffect
    {
        public readonly int Level;
        public int Time { get; set; }
        public readonly string Name;    // A custom name for the effect. Will default to class name when null.

        public virtual bool IsPermanent => false;

        public virtual string TypeName => GetType().Name.ToLower();     // Gets the true effect type name: i.e. the class name.
        public virtual string VariantName => Name ?? TypeName;          // Gets the custom name if present, otherwise the default name.
        public virtual string ApplyVerb => "affected with " + VariantName;
        public virtual string Color => "<purple>";

        public override string ToString() => VariantName + GetRomanLevel() + GetTimeString();
        public virtual string ToColorString() => Color + ToString().CapFirst() + "<r>";
        public string GetRomanLevel() => Level <= 1 ? "" : " " + Level.ToRoman();
        public virtual string GetTimeString() => IsPermanent ? "" : " (" + Time + ")";

        public StatusEffect(int level, int time, string name = null)
        {
            Level = level;
            Time = time;
            Name = name;
        }


        /// Saving & Loading:
        public StatusEffect(byte[] bytes, ref int pos)
        {
            // Level:
            Level = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Time:
            Time = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Name:
            var name = GameSaver.ToString(bytes, pos);
            Name = name != "" ? name : null;
            pos += 20;
        }

        public virtual byte[] ToBytes()
        {
            var bytes = new byte[32];
            int pos = 0;

            // First thing: status effect id
            bytes.Insert(pos, BitConverter.GetBytes(StatusEffectID.GetID(this)));
            pos += 4;

            // Level:
            bytes.Insert(pos, BitConverter.GetBytes(Level));
            pos += 4;

            // Time:
            bytes.Insert(pos, BitConverter.GetBytes(Time));
            pos += 4;

            // Name:
            bytes.Insert(pos, GameSaver.GetBytes(Name ?? ""));
            pos += 20;

            //Debug.WriteLine("saving: " + bytes.Stringify());
            return bytes;
        }

        public static StatusEffect Load(byte[] bytes, ref int pos)
        {
            //Debug.WriteLine("loading: " + bytes.ToList().GetRange(pos, 32).Stringify());

            // Get id & retrieve type from it.
            int id = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var type = StatusEffectID.GetType(id);

            if (!typeof(StatusEffect).IsAssignableFrom(type))
                return null;

            var se = (StatusEffect)Activator.CreateInstance(type, bytes, pos);

            pos += 28; // General SE bytes amount
            return se;
        }


        public virtual void Update(Entity entity)
        {
            Run(entity);
            
            if (Time > 0) Time--;
            if (!IsPermanent && Time <= 0)
            {
                entity.RemoveStatusEffect(this);
            }

        }

        public virtual void Run(Entity entity)
        {

        }
    }


    public abstract class StatusEffectOverTime : StatusEffect
    {
        protected virtual int GetApplyDelay() => 1;

        public StatusEffectOverTime(int level, int time, string name = null) : base(level, time, name) { }
        public StatusEffectOverTime(byte[] bytes, ref int pos) : base(bytes, ref pos) { }

        public override sealed void Run(Entity entity)
        {
            if (IsPermanent)
            {
                if (Time == 0)
                {
                    Apply(entity);
                    Time = GetApplyDelay();
                }
            }
            else
            {
                if (Time % GetApplyDelay() == 0)
                    Apply(entity);
            }
        }

        protected abstract void Apply(Entity entity);
    }
}
