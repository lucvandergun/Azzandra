
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class StairsUp : Instance
    {
        public override bool IsInteractable() => true;
        public override Symbol GetSymbol() { return new Symbol('<', Color.White); }
        public StairsUp(int x, int y) : base(x, y) { }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;
            if (!IsTouchingOrColliding(entity)) return;

            if (Program.Engine.LevelManager.GoToPreviousLevel())
            {
                player.User.Log.Add("<gold>You go up the stairs and ascend back to the previous level of the cavern.");
            }
            else
            {
                player.User.Log.Add("You cannot go up the stairs.");
            }
        }
    }


    public class StairsDown : Instance
    {
        public override bool IsInteractable() => true;
        public override Symbol GetSymbol() { return new Symbol('>', Color.Aqua); }
        public StairsDown(int x, int y) : base(x, y) { }

        public override void Interact(Entity entity)
        {
            if (!(entity is Player player)) return;
            if (!IsTouchingOrColliding(entity)) return;

            if (Program.Engine.LevelManager.GoToNextLevel())
            {
                player.User.Log.Add("<gold>You go down the stairs and descend to the next level of the cavern.");
            }
            else
            {
                player.User.Log.Add("You cannot go down the stairs.");
            }
        }
    }
}
