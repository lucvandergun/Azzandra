using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionPush : EntityAction
    {
        public readonly Vector Step;
        public int Amt;
        public bool HasSlided = true;
        
        public ActionPush(Entity caller, Vector step, int amt) : base(caller)
        {
            Step = step;
            Amt = amt;
            IsForced = true;
        }

        protected override bool PerformAction()
        {
            Caller.Move(Step, true, HasSlided);
            Amt--;
            if (Amt > 0)
                Caller.NextAction = this;
            return true;
        }

        public override string ToString()
        {
            return "Move: " + Amt + " x " + Step + ", hasSlided: " + HasSlided;
        }
    }
}
