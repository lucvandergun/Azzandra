using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class BlockMushroom : BlockData
    {
        public override bool IsInteractable() => true;

        public BlockMushroom(bool isWalkable, bool isAimable, bool isFlyable, bool isCornerable, bool blocksLight, int lightEmittance = 0)
            : base(isWalkable, isAimable, isFlyable, isCornerable, blocksLight, lightEmittance)
        {

        }

        public override void OnInstanceStep(Level level, BlockPos pos, Instance inst)
        {
            level.SetBlock(pos, new Block(BlockID.Void));
            if (inst is Player player)
                player.User.ShowMessage("You step on the mushroom.");
        }

        public override void OnInteraction(Level level, BlockPos pos, Entity entity)
        {
            // Place mushroom item in players inventory & remove block
            if (!(entity is Player player))
                return;

            var item = Item.Create("mushroom");
            if (player.User.Inventory.CanAddItem(item))
            {
                player.User.Inventory.AddItem(item);
                player.User.ShowMessage("You pick the mushroom.");
                level.RemoveBlock(pos);
            }
            else player.User.ShowMessage("You don't have enough space in your inventory to hold that.");
        }
    }
}
