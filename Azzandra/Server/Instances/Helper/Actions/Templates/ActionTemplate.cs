using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class ActionTemplate
    {
        public string Name = "action";
        public Func<Enemy, bool> Requirement;

        public bool CanBePerformed(Enemy actor) => Requirement?.Invoke(actor) ?? true;

        public ActionTemplate()
        {

        }

        public abstract EntityAction ToAction(Enemy caller);

        public override string ToString() => "Action template";
    }
}
