using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Affect
    {
        public readonly Server Server;
        public readonly int Speed = 1, Range = 1;
        public List<AttackProperty> Properties { get; set; } = new List<AttackProperty>();

        public List<string> Messages = new List<string>(2);

        public HitType HitType = HitType.Null;
        public bool IsSuccess() => HitType == HitType.Success;
        public bool IsFailed() => HitType == HitType.Failed || HitType == HitType.Null;
        protected abstract int GetPropertyAccuracy();


        public Affect(Server server, int speed, int range, List<AttackProperty> properties = null)
        {
            Server = server;
            Speed = speed;
            Range = range;
            Properties = properties;
        }

        /// <summary>
        /// Checks whether the Affect should be applied. Must be overridden (e.g. roll accuracy vs defense for attacks).
        /// </summary>
        public abstract bool RollSuccess(Entity attacker, Instance target);

        /// <summary>
        /// This method sets the Affect's values to be failed.
        /// </summary>
        public virtual void Fail(string msg)
        {
            AddMessage(msg);
            HitType = HitType.Failed;
        }

        /// <summary>
        /// Applies the Affect to the target
        /// </summary>
        public abstract void Apply(Entity attacker, Instance target);


        // === Property Handling === \\

        public bool HasProperty(int id)
        {
            if (Properties == null) return false;

            var type = AttackPropertyID.GetType(id);
            return Properties.Any(p => p.GetType() == type);
        }

        public bool TryGetProperty(int id, out AttackProperty prop)
        {
            var type = AttackPropertyID.GetType(id);
            prop = Properties.Find(p => p.GetType() == type);
            return prop != null;
        }

        /// <summary>
        /// This method adds an attack property, overriding existing same-ID lower level properties.
        /// </summary>
        public void AddProperty(AttackProperty a)
        {
            if (Properties == null)
                Properties = new List<AttackProperty>();

            for (int i = 0; i < Properties.Count; i++)
            {
                if (Properties[i].GetType() == a.GetType())
                {
                    if (Properties[i].Level < a.Level)
                        Properties[i] = a;
                    return;
                }
            }

            Properties.Add(a);
        }

        //public bool RemoveProperty(int id)
        //{
        //    if (TryGetProperty(id, 1, out var p))
        //    {
        //        Properties.Remove(p);
        //        return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Applies any properties of the Affect to the target.
        /// </summary>
        public virtual void ApplyProperties(Entity attacker, Instance target)
        {
            if (!(target is Entity cbtarget))
                return;

            if (Properties == null)
                return;

            foreach (var p in Properties)
            {
                if (p == null) return;
                
                if (Util.Random.NextDouble() >= p.ApplyChance)
                {
                    target.Level.Server.ThrowDebug(" Property [" + p.ToDebugString() + "] failed with apply chance of: " + p.ApplyChance);
                    continue;
                }

                // TODO: check whether effect is considered 'bad' for specific entity (based on entitytype). Though most are universally, if so.
                if (p.RollAccuracy && !Util.RollAgainst(GetPropertyAccuracy(), cbtarget.GetRes().Round()))
                {
                    target.Level.Server.ThrowDebug(" Property [" + p.ToDebugString() + "] failed roll of: acc: " + GetPropertyAccuracy() + ", vs res: " + cbtarget.GetRes());
                    continue;
                }

                var msg = p.Apply(attacker, cbtarget, this, false);
                if (msg != null)
                    AddMessage(msg);
                else
                    target.Level.Server.ThrowDebug("  - failed.");
            }
        }




        // === Message Handling === \\

        public virtual void AddMainMessage(Entity attacker, Instance target)
        {

        }

        /// <summary>
        /// Adds a message to the message list.
        /// </summary>
        public void AddMessage(string msg)
        {
            if (msg != null)
                Messages.Add(msg);
        }

        /// <summary>
        /// Displays all the current messages to the Server.User's log.
        /// </summary>
        public void ShowMessages()
        {
            // Add attack messages to log
            foreach (var msg in Messages)
            {
                Server.User.ShowMessage(msg, true);
            }
        }

        public virtual Hit CreateHitDisplay(Entity target)
        {
            return null;
        }
    }
}
