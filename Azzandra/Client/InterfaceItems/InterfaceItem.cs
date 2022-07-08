using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class InterfaceItem
    {
        public Func<InterfaceItem> NextItem { get; set; } // Tab-key will pass on to:

        public void OnTabKey()
        {
            SetUnFocussed();
            NextItem?.Invoke()?.SetFocussed();
        }

        public virtual void OnEnterKey()
        {
            OnTabKey();
        }

        public bool IsFocussed { get; protected set; } = false;
        protected bool JustFocussed = true;
        public virtual void SetFocussed()
        {
            IsFocussed = true;
            JustFocussed = true;
        }

        public virtual void SetUnFocussed()
        {
            IsFocussed = false;
            JustFocussed = true;
        }

        /// <summary>
        /// Call this first thing from any render method.
        /// </summary>
        public void UpdateKeyInput()
        {
            if (JustFocussed)
            {
                JustFocussed = false;
                return;
            }  

            if (Input.IsKeyPressed[Keys.Enter])
            {
                OnEnterKey();
            }
            else if (Input.IsKeyPressed[Keys.Tab])
            {
                OnTabKey();
            }
        }
    }
}
