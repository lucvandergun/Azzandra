using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class BlockCobweb : BlockData
    {
        public override bool IsInteractable() => true;

        public BlockCobweb(bool isWalkable, bool isAimable, bool isFlyable, bool isCornerable, bool blocksLight, int lightEmittance = 0)
            : base(isWalkable, isAimable, isFlyable, isCornerable, blocksLight, lightEmittance)
        {

        }

        public override void OnInteraction(Level level, BlockPos pos, Entity entity)
        {
            var player = (entity is Player p) ? p : null;
            
            if (Util.Random.Next(3) == 0)
            {
                level.RemoveBlock(pos);
                player?.User.Log.Add("You slash the cobweb.");
            }
            else
                player?.User.Log.Add("You fail to slash the cobweb.");
        }
    }
}
