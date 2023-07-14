using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Spectre : Enemy
    {
        public override EntityType EntityType => EntityType.Spirit;
        public override MoveType StartingMoveType => MoveType.Fly;

        public override int WanderRange => 2;

        public Spectre(int x, int y) : base(x, y) { }


        public override bool CanWalkOverBlock(Block block)
        {
            return true;
        }

        public override bool CanSeeThroughBlock(Block block)
        {
            return Target != null || base.CanSeeThroughBlock(block);
        }

        protected override bool CanTargetTile(Tile tile)
        {
            switch (tile.Ground.ID)
            {
                case BlockID.Wall:
                    return false;
            }

            return true;
        }

        public override Symbol GetSymbol() => new Symbol('s', Color.GreenYellow);
    }
}
