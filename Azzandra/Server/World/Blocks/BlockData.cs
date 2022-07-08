using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class BlockData
    {
        public bool IsWalkable { get; protected set; }
        public bool IsAimable { get; protected set; }
        public bool IsFlyable { get; protected set; }
        public bool IsCornerable { get; protected set; }
        public bool BlocksLight { get; protected set; }
        public int LightEmittance { get; protected set; }

        public virtual bool IsInteractable() => false;

        public BlockData(bool isWalkable, bool isAimable, bool isFlyable, bool isCornerable, bool blocksLight, int lightEmittance = 0)
        {
            IsWalkable = isWalkable;
            IsAimable = isAimable;
            IsFlyable = isFlyable;
            IsCornerable = isCornerable;
            BlocksLight = blocksLight;
            LightEmittance = lightEmittance;
        }


        // == Event Handlers == \\
        public virtual void OnInstanceStep(Level level, BlockPos pos, Instance inst)
        {

        }

        public virtual void OnInstanceStanding(Level level, BlockPos pos, Instance inst)
        {

        }

        public virtual void OnInteraction(Level level, BlockPos pos, Entity entity)
        {

        }
    }
}
