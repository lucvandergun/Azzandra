using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.TargetingMode
{
    public class TileTargeting : TargetingMode
    {
        public Vector? TileTarget { get; set; } = null;
        //public Vector SetTarget(Vector pos) => TileTarget = pos;

        public override bool HasTarget(Server server) => true;
        public override string GetActionString(Server server) => InboundAction is ActionVectorSpell ? "cast" : InboundAction is ActionThrow ? "throw" : "-";

        private int MoveTimer = 0;

        public TileTargeting() : base()
        {

        }


        public override void CheckSwitchTarget(InputHandler ih)
        {
            if ((MoveTimer <= 0 || ih.JustPressed) && ih.Dir != null)
            {
                if (TileTarget == null)
                    TileTarget = Vector.Zero;
                TileTarget += ih.Dir.ToVector();
                MoveTimer = ih.JustPressed ? 20 : 4;
            }

            if (MoveTimer > 0) MoveTimer--;
        }

        public override void CheckPerformAction(InputHandler ih)
        {
            if (Input.IsKeyPressed[Keys.Space])
            {
                PerformTargetAction(ih);
                return;
            }
        }

        public override void PerformTargetAction(InputHandler ih)
        {
            if (TileTarget != null)
            {
                if (InboundAction is ActionVectorSpell vs)
                {
                    var target = (TileTarget.Value + ih.Server?.User.Player?.Position).Value;

                    // Check target visible and in range
                    if (!ih.Server.User.Player.CanSee(target))
                    {
                        ih.Server.User.ShowMessage("<rose>You cannot see that point you want to cast at!");
                        return;
                    }
                        
                    if (ih.Server.User.Player.DistanceTo(target).ChebyshevLength() > 6)
                    {
                        ih.Server.User.ShowMessage("<rose>That point is out of range.");
                        return;
                    }  

                    vs.Target = target;
                    ih.Server.SetPlayerAction(vs);
                }

                else if (InboundAction is ActionVector av)
                {
                    var target = (TileTarget.Value + ih.Server?.User.Player?.Position).Value;
                    av.Target = target;
                    ih.Server.SetPlayerAction(av);
                }

                ih.TargetingMode = ih.DefaultTargetingMode;
            }
            else
            {
                ih.Server.User.ShowMessage("<rose>You don't have a target selected.");
            }
        }
    }
}
