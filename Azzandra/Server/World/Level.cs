using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public enum KeyType { Iron, Golden, Silver, Brass, Chrome, Nickel, }

    public class Level
    {
        public readonly Server Server;
        public readonly LevelManager LevelManager;

        public readonly int Depth;
        public readonly Temp Temp;
        public readonly int Seed;

        public readonly Random Random;
        public List<Generation.Area> Areas { get; set; }
        public int[,] AreaReferences { get; set; }


        // == Difficulty & Benefit Handling === \\
        public int DifficultyPoints { get; set; }
        public int DifficultyPointsUsed { get; set; }
        public int GetDifficultyPointsRemaining => DifficultyPoints - DifficultyPointsUsed;

        public int BenefitPoints => LevelManager.BenefitValue;  // Returns how many points are left to be used.
        public int RemoveBenefit(int amt) => LevelManager.RemoveBenefit(amt);   // Removes a certain amount of benefit from the total remaining benefit value.

        public Generation.Area GetAreaFromID(int id)
        {
            if (id < 0 || id >= Areas.Count)
                return null;

            else return Areas[id];
        }

        public Generation.Area GetArea(int x, int y)
        {
            if (!IsInMapBounds(x, y))
                return null;

            return GetAreaFromID(AreaReferences[x, y]);
        }

        public int MapWidth, MapHeight;
        public Tile[,] TileMap { get; set; }
        public Tile[,] MemoryTileMap { get; private set; }
        public float[,] StaticLightMap { get; private set; } // Map of tile-lightness (no instance lightsources yet)
        public float[,] LightMap { get; set; } // Full map of tile-lightness (with instance lightsources)

        public List<Instance> ActiveInstances { get; private set; }

        public Vector StartPosition { get; set; }
        public Vector EndPosition { get; set; }


        public Level(Server server, int depth, Temp temp, int seed)
        {
            // Setup
            Server = server;
            LevelManager = Server.LevelManager;
            Depth = depth;
            Temp = temp;
            Seed = seed;// != null ? seed.Value : CreateRandomSeed();
            Random = new Random(Seed);

            ActiveInstances = new List<Instance>();
        }

        public void Generate()
        {
            // Delete all active instances - useful when re-generating the world with a world present
            ActiveInstances.Clear();

            // Generate map:
            var generator = Depth < 30 ? new Generation.Generator(this) : new Generation.GeneratorBossLevel(this);
            generator.GenerateLevel();

            // Fill and create memory tile map
            MemoryTileMap = new Tile[MapWidth, MapHeight];
            MemoryTileMap.Populate(new Tile(BlockID.Void));
            SetupLightMap();
        }

        /// <summary>
        /// Creates the specified instance: giving it a unique instance ID, initialising (.Init()) the instance and adding it to the active list.
        /// </summary>
        /// <param name="inst">The instance to create.</param>
        /// <returns>The instance created.</returns>
        public Instance CreateInstance(Instance inst)
        {
            inst.ID = LevelManager.GetUniqueInstanceID();
            //Server.User.ShowMessage("Assigned id: " + inst.ID);
            AddInstance(inst);
            inst.Init();
            return inst;
        }

        /// <summary>
        /// Adds the specified (and fully completed) instance to the active list.
        /// Does NOT assign instance ID!
        /// </summary>
        /// <param name="inst">The instance to add.</param>
        /// <returns>The instance added.</returns>
        public Instance AddInstance(Instance inst)
        {
            ActiveInstances.Add(inst);
            inst.Level = this;
            return inst;
        }

        public bool CanCreateInstance(Instance inst)
        {
            inst.Level = this;

            // Check whether instance can exist here:
            return inst.CanExist(inst.X, inst.Y);
        }

        public void RemoveInstance(Instance inst)
        {
            ActiveInstances.Remove(inst);
        }

        public Instance GetInstanceByID(int id)
        {
            var inst = ActiveInstances.FirstOrDefault(i => i.ID == id);
            return inst;
        }


        public bool IsInMapBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
        }

        public void MarkTile(Vector v, int id)
        {
            if (IsInMapBounds(v.X, v.Y))
                TileMap[v.X, v.Y].Marker = id;
        }

        public Tile GetTile(Vector v) => GetTile(v.X, v.Y);
        public Tile GetTile(int x, int y)
        {
            return IsInMapBounds(x, y) ?  TileMap[x, y] : new Tile(BlockID.Void);
        }
        public Block GetBlock(BlockPos pos)
        {
            if (IsInMapBounds(pos.Position.X, pos.Position.Y))
            {
                var tile = TileMap[pos.Position.X, pos.Position.Y];
                return pos.IsFloor ? tile.Ground : tile.Object;
            }

            return new Block(BlockID.Void);
        }

        public void SetTile(Vector v, Tile tile)
        {
            if (IsInMapBounds(v.X, v.Y))
                TileMap[v.X, v.Y] = tile;
        }
        public void SetBlock(BlockPos pos, Block block)
        {
            if (IsInMapBounds(pos.Position.X, pos.Position.Y))
            {
                if (pos.IsFloor)
                    TileMap[pos.Position.X, pos.Position.Y].Ground = block;
                else
                    TileMap[pos.Position.X, pos.Position.Y].Object = block;
            }
        }
        public void RemoveBlock(BlockPos pos)
        {
            if (IsInMapBounds(pos.Position.X, pos.Position.Y))
            {
                if (pos.IsFloor)
                    TileMap[pos.Position.X, pos.Position.Y].Ground = new Block(BlockID.Void);
                else
                    TileMap[pos.Position.X, pos.Position.Y].Object = new Block(BlockID.Void);
            }
        }
        public void SetGround(Vector v, Block block, bool overrideFilled = false)
        {
            if (IsInMapBounds(v.X, v.Y) && (TileMap[v.X, v.Y].Ground.ID == BlockID.Floor || overrideFilled))
                TileMap[v.X, v.Y].Ground = block;
        }
        public void SetObject(Vector v, Block block, bool overrideFilled = false)
        {
            if (IsInMapBounds(v.X, v.Y) && (TileMap[v.X, v.Y].Object.ID == BlockID.Void || overrideFilled))
                TileMap[v.X, v.Y].Object = block;
        }

        public BlockPos? BlockCheckPosition(Vector pos, bool reqSolidity)
        {
            // Returns requested block if meets requirements. (First checks object, then ground.)
            if (!GetTile(pos).Object.Data.IsWalkable || !reqSolidity)
                return new BlockPos(pos, false);
            else if (!GetTile(pos).Ground.Data.IsWalkable || !reqSolidity)
                return new BlockPos(pos, true);
            else
                return null;
        }


        public void TickStart(bool isPlayerTick)
        {
            //Debug.WriteLine("tstart: " +ActiveInstances.Where(i => i.IsOnPlayerTick == isPlayerTick).Stringify());
            Instance inst;
            for (int i = 0; i < ActiveInstances.Count; i++)
            {
                inst = ActiveInstances[i];
                
                if (inst.IsOnPlayerTick == isPlayerTick)
                {
                    //if (!inst.IsDestroyed)
                    //    inst.TickStart();

                    //// Remove instance if it's destroyed.
                    //if (inst.IsDestroyed)
                    //    ActiveInstances.RemoveAt(i);

                    inst.TickStart();

                    // If instance has been removed, it's dead: move on.
                    if (i < ActiveInstances.Count && inst != ActiveInstances[i])
                    {
                        i--; continue;
                    }
                } 
            }
        }

        public void Tick(bool isPlayerTick)
        {
            //Debug.WriteLine("tick: " + ActiveInstances.Where(i => i.IsOnPlayerTick == isPlayerTick).Stringify());
            Instance inst;
            for (int i = 0; i < ActiveInstances.Count; i++)
            {
                inst = ActiveInstances[i];

                if (inst.IsOnPlayerTick == isPlayerTick)
                {
                    inst.Tick();
                }
            }
        }

        public void TickEnd(bool isPlayerTick)
        {
            //Debug.WriteLine("tend: " + ActiveInstances.Where(i => i.IsOnPlayerTick == isPlayerTick).Stringify());
            Instance inst;
            for (int i = 0; i < ActiveInstances.Count; i++)
            {
                inst = ActiveInstances[i];

                if (inst.IsOnPlayerTick == isPlayerTick)
                {
                    inst.TickEnd();

                    // If instance has been removed, it's dead: move on.
                    if (i < ActiveInstances.Count && inst != ActiveInstances[i])
                    {
                        i--; continue;
                    }
                }
            }
        }

        public Instance InstanceCheckPosition(int x, int y, Instance self, bool reqSolidity)
        {
            Instance bestMatch = null;

            //returns first instance that meets requirements.
            foreach (var inst in ActiveInstances)
            {
                if (!inst.IsSolid() && reqSolidity)
                    continue;

                //if (inst != self && inst.X <= x && inst.X + inst.GetW() > x && inst.Y <= y && inst.Y + inst.GetH() > y)
                if (inst != self && inst.GetTiles().Contains(new Vector(x, y)))
                {
                    if (inst is Entity)
                        return inst;
                    else if (bestMatch == null && inst.CanBeTargetedByPlayer())
                        bestMatch = inst;
                }
            }

            return bestMatch;
        }

        public bool CheckInstanceExists(Instance inst)
        {
            foreach (var i in ActiveInstances)
            {
                if (i == inst) return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates whether the given instance is in visible light: any of the tiles occupying it are on a tile with lighness > 0.
        /// </summary>
        /// <param name="inst">The instance to check.</param>
        /// <returns>Whether the instance is in visible light.</returns>
        public bool IsInstanceVisible(Instance inst)
        {
            return inst.GetTiles().Any(t => LightMap[t.X, t.Y] > 0);
        }


        public void ReQueueInstances(Instance centerInst)
        {
            var amtOfInstances = ActiveInstances.Count;
            var newList = new List<Instance>(amtOfInstances);
            var distList = new List<int>(amtOfInstances);

            int dist;
            foreach (var inst in ActiveInstances)
            {
                dist = inst.TileDistanceTo(centerInst);

                // Find list pos
                var count = distList.Count;
                for (int i = 0; i <= count; i++)
                {
                    if (i == count)
                    {
                        newList.Add(inst);
                        distList.Add(dist);
                        break;
                    }
                    else if (dist < distList[i])
                    {
                        newList.Insert(i, inst);
                        distList.Insert(i, dist);
                        break;
                    }
                }
            }


            ActiveInstances = newList;
        }

        public void UpdateMemoryTileMap(bool[,] visibilityMap)
        {
            // Add new visible tiles to memory tile map
            for (int i, j = 0; j < MapHeight; j++)
            {
                for (i = 0; i < MapWidth; i++)
                {
                    // Edit memory tile if tile is currently visible: in sight line AND light level > 0
                    if (visibilityMap[i, j] && LightMap[i, j] > 0f)
                    {
                        // If memory tile is not yet defined or outdated:
                        if (MemoryTileMap[i, j] == null || MemoryTileMap[i, j] != TileMap[i, j])
                        {
                            MemoryTileMap[i, j] = TileMap[i, j];
                        }
                    }
                }
            }
        }

        public Tile GetMemoryTile(int x, int y)
        {
            return IsInMapBounds(x, y) ? MemoryTileMap[x, y] : new Tile(BlockID.Void);
        }

        private void SetupLightMap()
        {
            /* Potentialities:
             * Level.AddLight(..);
             * Level.AddTileLightSources();
             * lavaBody.EmitLight();
             */

            StaticLightMap = new float[MapWidth, MapHeight];
            LightMap = new float[MapWidth, MapHeight];

            int light;
            for (int i, j = 0; j < MapHeight; j++)
            {
                for (i = 0; i < MapWidth; i++)
                {
                    light = TileMap[i, j].GetLightEmittance();
                    if (light != 0)
                    {
                        var pos = new Vector(i, j);
                        StaticLightMap.AddLight(pos, LightLevelCalculator.EmitLight(this, pos, light));
                    }
                }
            }
        }

        public float GetTileLightness(Vector v)
        {
            if (IsInMapBounds(v.X, v.Y))
                return LightMap[v.X, v.Y];
            else
                return 0f;
        }

        public bool NodeBlocksLight(Vector n)
        {
            if (!IsInMapBounds(n.X, n.Y))
                return false;

            var tile = TileMap[n.X, n.Y];
            if (tile.Ground.Data.BlocksLight || tile.Object.Data.BlocksLight)
                return true;

            if (ActiveInstances.Exists(i => i.BlocksLight() && i.GetTiles().Contains(n)))
                return true;

            return false;
        }



        // == Saving & Loading == \\

        public byte[] ToBytes()
        {
            var bytes = new byte[8 + 16];

            // Save mapsize
            bytes.Insert(0, BitConverter.GetBytes(MapWidth));
            bytes.Insert(4, BitConverter.GetBytes(MapHeight));
            bytes.Insert(8, BitConverter.GetBytes(StartPosition.X));
            bytes.Insert(12, BitConverter.GetBytes(StartPosition.Y));
            bytes.Insert(16, BitConverter.GetBytes(EndPosition.X));
            bytes.Insert(20, BitConverter.GetBytes(EndPosition.Y));

            // Save tilemap
            bytes = bytes.Concat(SaveTileMap(TileMap)).ToArray();

            // Save memory tilemap
            bytes = bytes.Concat(SaveTileMap(MemoryTileMap)).ToArray();

            // Save instances - Amt of instances is added before them
            bytes = bytes.Concat(SaveInstances()).ToArray();

            return bytes;
        }

        private byte[] SaveTileMap(Tile[,] tiles)
        {
            var bytes = new byte[MapWidth * MapHeight * 2 * 4];
            int pos = 0;
            Tile tile;
            for (int x, y = 0; y < MapHeight; y++)
            {
                for (x = 0; x < MapWidth; x++)
                {
                    tile = tiles[x, y];
                    bytes.Insert(pos, BitConverter.GetBytes(tile.Ground.ID));
                    pos += 4;
                    bytes.Insert(pos, BitConverter.GetBytes(tile.Object.ID));
                    pos += 4;
                }
            }

            return bytes;
        }

        private byte[] SaveInstances()
        {
            var bytes = new byte[0];

            var instances = ActiveInstances.CreateCopy();
            instances.RemoveAll(p => p is Player);              // Don't save player, it is saved through User.

            int amt = instances.Count;

            foreach (var inst in instances)
            {
                //Debug.WriteLine(" SAVING INSTANCE: " + inst.Name);

                // First construct a header (inst type + inst bytes amt) [24 bytes], then store the instance bytes [variable bytes]
                var header = new byte[24];
                header.Insert(0, GameSaver.GetBytes(inst.GetTypeID()));
                
                var instBytes = inst.ToBytes();
                var instBytesAmt = instBytes.Length;    //get and add inst bytes amt
                header.Insert(20, BitConverter.GetBytes(instBytesAmt));

                instBytes = header.Concat(instBytes).ToArray();

                // Add instance-specific bytes to the level total array
                bytes = bytes.Concat(instBytes).ToArray();
                
            }

            return BitConverter.GetBytes(amt).Concat(bytes).ToArray();
        }

        public void Load(byte[] bytes)
        {
            int pos = 0;

            // Load mapsize
            MapWidth = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            MapHeight = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            
            int x, y;
            x = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            y = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            StartPosition = new Vector(x, y);
            x = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            y = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            EndPosition = new Vector(x, y);

            // Load tilemap
            TileMap = LoadTileMap(bytes, ref pos);

            // Load memory tilemap
            MemoryTileMap = LoadTileMap(bytes, ref pos);

            SetupLightMap();

            // Load instances
            int amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            LoadInstances(bytes, ref pos, amt);
        }

        private Tile[,] LoadTileMap(byte[] bytes, ref int pos)
        {
            var tiles = new Tile[MapWidth, MapHeight];
            int id1, id2;
            
            for (int x, y = 0; y < MapHeight; y++)
            {
                for (x = 0; x < MapWidth; x++)
                {
                    id1 = BitConverter.ToInt32(bytes, pos);
                    pos += 4;
                    id2 = BitConverter.ToInt32(bytes, pos);
                    pos += 4;
                    tiles[x, y] = new Tile(id1, id2);
                }
            }

            return tiles;
        }

        private void LoadInstances(byte[] bytes, ref int pos, int amt)
        {
            Instance inst;

            for (int i = 0; i < amt; i++)
            {
                // Get inst type id
                var instTypeID = GameLoader.ToString(bytes, pos);
                pos += 20;

                // Get amt of bytes for inst
                var instBytesAmt = BitConverter.ToInt32(bytes, pos);
                pos += 4;
                
                // Get inst bytes: copy subarray
                var instBytes = new byte[instBytesAmt];
                Array.Copy(bytes, pos, instBytes, 0, instBytesAmt);
                pos += instBytesAmt;

                // Create instance from bytes
                // Debug.WriteLine(" LOADING INSTANCE: " + instTypeID);
                inst = GameLoader.LoadInstance(instTypeID, instBytes);
                if (inst != null)
                {
                    AddInstance(inst);
                }
            }
        }

        /// <summary>
        /// Used after loading ALL instances (including player) to update any loaded InstRefs to these actual instances.
        /// </summary>
        public void LoadInstRefs()
        {
            // Update instrefs:
            foreach (var instRef in InstRef.LoadedReferences)
            {
                instRef.Instance = ActiveInstances.FirstOrDefault(i => i.ID == instRef.ID);
            }
            InstRef.LoadedReferences.Clear();
        }
    }
}
