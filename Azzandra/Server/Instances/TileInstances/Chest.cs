using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Chest : Instance
    {
        private bool IsOpen = false;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return IsOpen ? new Symbol('c', Color.Orange)
                : new Symbol('¢', Color.Orange);
            
            //¢
        }

        public Chest(int x, int y, bool isOpen = false) : base(x, y)
        {
            IsOpen = isOpen;
        }

        public Chest(int x, int y) : base(x, y)
        {
            IsOpen = false;
        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;
            if (!entity.IsTouching(this)) return;


            if (!IsOpen)
            {
                IsOpen = true;
                player.User.Log.Add("<gold>You open the chest.");
                return;
            }

            player.User.Log.Add("The chest appears to be empty.");

            //else open chest interface
        }
    }
}
