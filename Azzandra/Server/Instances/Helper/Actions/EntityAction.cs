using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class EntityAction
    {
        protected readonly Entity Caller;
        public bool IsForced = false;

        // Action delay handling: not effectively used as of now.
        protected virtual int Delay => 1;
        protected int Timer = 0;

        public EntityAction(Entity caller)
        {
            Caller = caller;
        }


        /// <summary>
        /// Performs the action.
        /// </summary>
        /// <returns>Whether this action was succesful: i.e. the player should have their turn passed.</returns>
        public virtual bool Perform()
        {
            Timer++;
            
            if (Timer >= Delay)
            {
                Timer = 0;
                return PerformAction();
            }
            else
            {
                Caller.NextAction = this;
            }

            return true;
        }

        /// <summary>
        /// Performs this specific action action.
        /// </summary>
        /// <returns>Whether this action was succesful: i.e. the player should have their turn passed.</returns>
        protected abstract bool PerformAction();

        public override string ToString() => GetType().Name;
    }
}
