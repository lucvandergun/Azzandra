using System.Collections.Generic;

namespace Azzandra
{
    public static class BlockID
    {
        public const int
            // Ground:
            Void = -1,
            Floor = 0,
            Wall = 1,
            Water = 2,
            Lava = 3,
            Chasm = 4,
            Dock = 5,
            Bricks = 6,
            Ice = 7,
            Poison = 8,
            Sulphur = 9,
            Mud = 10,

            // Object:
            Pillar = 100,
            Rock = 101,
            Crystal = 102,
            IronOre = 103,
            Coal = 104,
            Mushroom = 105,
            Torch = 106,
            Cobweb = 107,
            LightMushroom = 108,
            Plant = 109,
            Vine = 110,
            Root = 111,
            Icicle = 112,
            Stool = 113,
            Table = 114,
            Acid = 115,
            Rubble = 116;


        private static readonly Dictionary<int, BlockData> BlockRegistry = new Dictionary<int, BlockData>();
        public static void LoadData()
        {
            BlockRegistry.Add(Void, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Floor, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Wall, new BlockData(false, false, false, false, true, 0));
            BlockRegistry.Add(Water, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Lava, new BlockData(false, true, true, true, false, 8));
            BlockRegistry.Add(Chasm, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Dock, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Bricks, new BlockData(false, false, false, false, true, 0));
            BlockRegistry.Add(Ice, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Poison, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Sulphur, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Mud, new BlockData(true, true, true, true, false, 0));
            
            BlockRegistry.Add(Pillar, new BlockData(false, false, false, true, false, 0));
            BlockRegistry.Add(Rock, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Crystal, new BlockData(false, true, true, true, false, 4));
            BlockRegistry.Add(IronOre, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Coal, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Mushroom, new BlockMushroom(true, true, true, true, false, 0));
            BlockRegistry.Add(LightMushroom, new BlockData(false, true, true, true, false, 4));
            BlockRegistry.Add(Torch, new BlockData(false, true, true, true, false, 6));
            BlockRegistry.Add(Cobweb, new BlockCobweb(true, true, true, true, false, 0));
            BlockRegistry.Add(Plant, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Vine, new BlockVine(true, true, true, true, false, 0));
            BlockRegistry.Add(Root, new BlockRoot(false, true, false, true, false, 0));
            BlockRegistry.Add(Icicle, new BlockIcicle(false, true, true, true, false, 0));
            BlockRegistry.Add(Stool, new BlockData(false, true, true, true, false, 0));
            BlockRegistry.Add(Table, new BlockData(false, true, true, false, false, 0));
            BlockRegistry.Add(Acid, new BlockData(true, true, true, true, false, 0));
            BlockRegistry.Add(Rubble, new BlockData(true, true, true, true, false, 0));
        }

        public static BlockData GetData(int tileID)
        {
            if (BlockRegistry.TryGetValue(tileID, out var data))
                return data;
            else
                return BlockRegistry[Void];
        }
    }
}
