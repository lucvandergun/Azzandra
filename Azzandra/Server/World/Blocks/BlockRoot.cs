using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class BlockRoot : BlockData
    {
        public override bool IsInteractable() => true;

        public BlockRoot(bool isWalkable, bool isAimable, bool isFlyable, bool isCornerable, bool blocksLight, int lightEmittance = 0)
            : base(isWalkable, isAimable, isFlyable, isCornerable, blocksLight, lightEmittance)
        {

        }

        public override void OnInteraction(Level level, BlockPos pos, Entity entity)
        {
            if (Util.Random.Next(3) == 0)
            {
                level.RemoveBlock(pos);
                if (entity is Player player)
                    player.User.ShowMessage("You destroy the roots.");
            }
            else if (entity is Player player)
                player.User.ShowMessage("You fail to destroy the roots.");

        }
    }
}
