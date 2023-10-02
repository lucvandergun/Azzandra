using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class TemplateAffect : ActionTemplate
    {
        public int Speed = 1, Range = 1;
        public Func<Enemy, Instance> TargetSelector = new Func<Enemy, Entity>(e => e.Target.Entity);

        public virtual bool IsInRange(int range) => Range >= range;

        public TemplateAffect() { }

        public TemplateAffect(int speed, int range)
        {
            Speed = speed;
            Range = range;
        }

        public abstract Affect ToAffect(Server server);

        public override EntityAction ToAction(Enemy caller)
        {
            return caller.CreateActionForAffect(TargetSelector?.Invoke(caller), ToAffect(caller.Level.Server));
        }

        public override string ToString() => "Affect template";
    }
}
