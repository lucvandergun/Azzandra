using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Grave : Instance
    {
        private bool IsEmpty = false;
        public override bool IsInteractable() => true;

        public override Symbol GetSymbol()
        {
            return IsEmpty ? new Symbol('G', new Color(64, 64, 64))
                : new Symbol('G', Color.Gray);
        }

        public Grave(int x, int y) : base(x, y)
        {

        }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;
            if (!entity.IsTouching(this)) return;

            player.User.Log.Add("You examine the grave, it reads:");
            player.User.Log.Add("<gold> Here lies miner X, R.I.P.");
        }
    }
}
