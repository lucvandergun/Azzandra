using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Barrel : Instance
    {
        private bool IsOpen = false;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('U', Color.Brown)
                : new Symbol('U', Color.Brown);
        }

        public Barrel(int x, int y, bool isOpen = false) : base(x, y)
        {
            IsOpen = isOpen;
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;
            if (!entity.IsTouching(this)) return;


            if (!IsOpen)
            {
                IsOpen = true;
                player.User.Log.Add("<gold>You open the barrel.");
                return;
            }

            player.User.Log.Add("The barrel is empty.");
        }
    }
}
