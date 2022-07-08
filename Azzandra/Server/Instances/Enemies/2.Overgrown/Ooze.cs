using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Ooze : Enemy
    {
        public override EntityType EntityType => EntityType.Acid;
        public override int GetMoveDelay() => 2;

        public Ooze(int x, int y) : base(x, y) { }

        public override void TickStart()
        {
            base.TickStart();

            // Add acid to the floor
            Level.SetObject(Position, new Block(BlockID.Acid));
        }

        public override EntityAction DetermineRegularAction()
        {
            return null;
        }


        public override Symbol GetSymbol() => new Symbol("o", Color.YellowGreen);
    }
}
