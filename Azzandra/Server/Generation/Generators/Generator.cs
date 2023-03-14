using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Azzandra.Generation
{
    public class Generator
    {
        public Random Random;
        public Level Level { get; private set; }

        public int
            MapWidth = 40,
            MapHeight = 40;

        public int[,] IDMap;
        public Tile[,] TileMap;
        protected List<Area> Areas;
        protected List<Area> Lakes;
        protected Area Floor;

        public static readonly int WallDeath = 4, FloorDeath = 5;

        protected int
            CaveSmoothness = 5,
            LakeSmoothness = 5,
            MinCaveSize = 10,
            MinLakeSize = 10,
            CavePercent = 46,   // Initial percentage floor
            LakeIncorporationPercent = 67,   // Chance of lake to be kept
            LakeOverlapPercent = 50;    // Chance of lake to be allowed to overlap: not used

        protected int
            RoomMinAmount = 1,
            RoomMaxAmount = 5,
            MinRoomSize = 4,
            MaxRoomSize = 8,
            EnclosedPercent = 50;

        //public Vector GetRoomDimensions(Random random) =>
        //    new Vector(random.Next(MaxRoomSize - MinRoomSize + 1) + MinRoomSize, random.Next(MaxRoomSize - MinRoomSize + 1) + MinRoomSize);

        protected const int
            ROOM_ID = 1000;

        protected virtual void AssignPopulator(Level level) => new Populator(level).PopulateLevel();

        public Generator(Level level)
        {
            Level = level;
            Random = level.Random;
        }

        public void GenerateLevel()
        {
            GenerateAreaLayout();

            // Convert integer map to Tile map (Could slope wall & water tiles here)
            Level.TileMap = TileMap;
            Level.MapWidth = MapWidth;
            Level.MapHeight = MapHeight;
            Level.Areas = Areas;

            // Populate the level with the Populator:
            AssignPopulator(Level);
        }

        protected virtual void GenerateAreaLayout()
        {
            var temp = Level.Temp;

            // Create cavern tilemap:
            IDMap = CreateAreaPattern(MapWidth, MapHeight, CavePercent, CaveSmoothness);
            IDMap = ChangeAreas(IDMap, BlockID.Floor, BlockID.Wall, x => x.Nodes.Count() < MinCaveSize);

            // Add lakes:
            var lakeData = GetLakeTypes(temp);
            IDMap = GenerateLakes(IDMap, lakeData, LakeIncorporationPercent, MinLakeSize, MinCaveSize);

            // Add rooms:
            Areas = IdentifyAreas(IDMap, BlockID.Floor);
            var amountOfRooms = Random.Next(RoomMinAmount, RoomMaxAmount + 1);
            IDMap = GenerateRooms(IDMap, Areas, amountOfRooms, MinRoomSize, MaxRoomSize, EnclosedPercent);
            var rooms = Areas.FindAll(a => a is Room);

            // Split cavern areas after generation of rooms:
            IDMap = ChangeAreas(IDMap, BlockID.Floor, BlockID.Wall, x => x.Nodes.Count() < MinCaveSize);    // Change small caverns to wall tiles
            Areas = IdentifyAreas(IDMap, BlockID.Floor);
            IDMap = ChangeAreas(IDMap, ROOM_ID, BlockID.Floor);     // Revert room identifiers to floor tiles
            Areas.AddRange(rooms);

            // Shuffle areas and identify edge points:
            Areas.ForEach(a => a.IdentifyEdgePoints());
            Areas.Shuffle(Random);

            // Convert current (int) IDMap to a (Tile) TileMap:
            TileMap = ConvertToTileMap(IDMap);

            // Create connections and pathways between all areas:
            ConnectAreas(TileMap, Areas);

            // Inter-connect all non-connected rooms
            //ConnectNonConnectedRooms(IDMap, Areas);
        }

        /// <summary>
        /// Converts the integer map to a Tile map.
        /// The old type gets parsed to the Tile.Ground value.
        /// </summary>
        /// <param name="map">The integer type map</param>
        /// <returns>A Tile map of the same dimensions.</returns>
        protected Tile[,] ConvertToTileMap(int[,] map)
        {
            // Check the surroundings of each position to create the corresponding tile
            var tileMap = new Tile[MapWidth, MapHeight];
            for (int x, y = 0; y < MapHeight; y++)
                for (x = 0; x < MapWidth; x++)
                    tileMap[x, y] = CreateTileFromSurroundings(map, x, y);

            return tileMap;
        }

        private Tile CreateTileFromSurroundings(int[,] map, int x, int y)
        {
            var type = map[x, y];

            return new Tile(type);
        }

        protected int[,] GenerateLakes(int[,] map, List<LakeData> lakeData, int lakePercent, int minLakeSize, int minCaveSize)
        {
            // Create each tilemap & overlap it
            foreach (var lakeType in lakeData)
            {
                var lakeTileID = lakeType.Type;
                var density = lakeType.Density;

                // Create lake tilemap
                var lakeMap = CreateAreaPattern(MapWidth, MapHeight, density, LakeSmoothness);

                // Overlap lake map to existing cavern map
                map = OverlapMaps(map, lakeMap, lakeTileID);

                // Change small lakes and X% of lakes to walls
                map = ChangeAreas(map, lakeTileID, BlockID.Wall, a => a.Nodes.Count() < minLakeSize || Random.Next(0, 100) >= lakePercent);

                // Change small caves to current lake type
                map = ChangeAreas(map, BlockID.Floor, lakeTileID, a => a.Nodes.Count() < minCaveSize);
            }

            return map;
        }

        protected int[,] GenerateRooms(int[,] map, List<Area> areas, int amountOfRooms, int minRoomSize, int maxRoomSize, int enclosedPercentage)
        {
            //// Add rooms to floorplan
            //var roomAttempts = amountOfRooms * 10;

            //while (roomAttempts > 0 && amountOfRooms > 0)
            //{
            //    roomAttempts--;

            //    // Create a room dimensions
            //    int roomWidth = Random.Next(minRoomSize, maxRoomSize + 1);
            //    int roomHeight = Random.Next(minRoomSize, maxRoomSize + 1);
            //    int roomX = Random.Next(1, MapWidth - roomWidth);
            //    int roomY = Random.Next(1, MapHeight - roomHeight);

            //    // Check whether room overlaps lakes
            //    if (!CanRoomBeBuilt(roomX, roomY, roomWidth, roomHeight))
            //        continue;

            //    // Create room & add to areas
            //    bool isEnclosed = Random.Next(100) >= enclosedPercentage;
            //    var room = new Room(Level, roomX, roomY, roomWidth, roomHeight, isEnclosed);
            //    areas.Add(room);

            //    // Add room to the tilemap:
            //    map = AddRoomToMap(map, areas, room);

            //    amountOfRooms--;
            //}

            for (int i = 0; i < Random.Next(2) + 1; i++)
            {
                map = GenerateRoomStructure(map, areas, amountOfRooms, minRoomSize, maxRoomSize, enclosedPercentage);
            }

            return map;
        }

        protected int[,] GenerateRoomStructure(int[,] map, List<Area> areas, int amountOfRooms, int minRoomSize, int maxRoomSize, int enclosedPercentage)
        {
            // Add rooms to floorplan
            var roomAttempts = amountOfRooms * 10;
            var structure = new List<Room>();

            while (roomAttempts > 0 && structure.Count < amountOfRooms)
            {
                roomAttempts--;

                var room = AddRoomToStructure(map, areas, structure, minRoomSize, maxRoomSize, enclosedPercentage);

                // Failed to find room pos
                if (room == null)
                    continue;

                // Add room to the tilemap:
                structure.Add(room);
                areas.Add(room);
                map = AddRoomToMap(map, areas, room);

                amountOfRooms--;
            }

            return map;
        }

        protected Room AddRoomToStructure(int[,] map, List<Area> areas, List<Room> structure, int minRoomSize, int maxRoomSize, int enclosedPercentage)
        {
            // Create a room template:
            int roomWidth = Random.Next(minRoomSize, maxRoomSize + 1);
            int roomHeight = Random.Next(minRoomSize, maxRoomSize + 1);
            bool isEnclosed = Random.Next(100) >= enclosedPercentage;

            // If first room: place wherever
            if (structure.Count == 0)
            {
                var room = new Room(Level, Random.Next(1, MapWidth - roomWidth), Random.Next(1, MapHeight - roomHeight), roomWidth, roomHeight, isEnclosed);
                room.IdentifyEdgePoints();

                if (CanRoomBeBuilt(map, room))
                    return room;
            }

            // Not the first room: pick wall and connect there
            else
            {
                var otherArea = structure.PickRandom(Random);
                var node = otherArea.EdgeNodes.PickRandom();
                var positions = new List<Vector>()
                    {
                        new Vector(node.X + 1,                         node.Y - Random.Next(roomHeight - 1) + 1),
                        new Vector(node.X - Random.Next(roomWidth - 1) + 1,    node.Y + 1),
                        new Vector(node.X - roomWidth - 1,             node.Y - Random.Next(roomHeight - 1) + 1),
                        new Vector(node.X - Random.Next(roomWidth - 1) + 1,    node.Y - roomHeight - 1),
                    };

                positions.Shuffle(Random);
                foreach (var pos in positions)
                {
                    var room = new Room(Level, pos.X, pos.Y, roomWidth, roomHeight, isEnclosed);
                    room.IdentifyEdgePoints();

                    if (room.Nodes.Any(n => n.X <= 0 || n.Y <= 0 || n.X >= map.GetLength(0) - 1 || n.Y >= map.GetLength(1) - 1))
                        continue;

                    // Check whether room overlaps lakes
                    if (otherArea.EdgeNodes.Intersect(room.EdgeNodes).Count() > 0 && CanRoomBeBuilt(map, room))
                        return room;
                }
            }

            return null;
        }

        protected int[,] AddRoomToMap(int[,] map, List<Area> areas, Room room)
        {
            for (int i, j = room.Y - 1; j < room.Y + room.H + 1; j++)
            {
                for (i = room.X - 1; i < room.X + room.W + 1; i++)
                {
                    // If inside room: add floor tiles
                    if (i >= room.X && i < room.X + room.W && j >= room.Y && j < room.Y + room.H)
                    {
                        map[i, j] = ROOM_ID;
                        if (map[i, j] == BlockID.Floor || map[i, j] == ROOM_ID)
                            RemoveTileFromAllAreas(areas, room, new Vector(i, j));
                    }
                    // Else if on room border: add brick tiles
                    else if (room.IsEnclosed || map[i, j] == BlockID.Wall)
                    {
                        map[i, j] = BlockID.Bricks;
                        RemoveTileFromAllAreas(areas, room, new Vector(i, j));
                    }
                }
            }

            return map;
        }

        private void RemoveTileFromAllAreas(List<Area> areas, Area newArea, Vector tile)
        {
            for (int i = areas.Count() - 1; i >= 0; i--)
            {
                var area = areas[i];
                if (area != newArea && area.Nodes.Contains(tile))
                {
                    area.Nodes.Remove(tile);

                    // Remove area if it is too small (old = has no nodes left)
                    if (area.Nodes.Count() < MinCaveSize) //(area.Nodes.Count() <= 0)
                    {
                        Areas.Remove(area);
                        // TODO: set area to no tiles here... or dont do this here at all...
                    } 
                }
            }
        }


        protected int[,] CreateAreaPattern(int width, int height, int initialPercentage, int smoothness)
        {
            // Create and randomly fill the map (0s and 1s)
            var map = new int[width, height];
            map = RandomFillMap(map, initialPercentage, Random);

            // Smoothify the map x amount of times
            for (int amt = 0; amt < smoothness; amt++)
                map = SmoothifyMap(map);

            return map;
        }

        protected int[,] ChangeAreas(int[,] map, int oldTileID, int newTileID, Predicate<Area> filter = null)
        {
            // Identify the distinct areas (id: 0)
            var areas = IdentifyAreas(map, oldTileID);

            // Remove and fill fill the areas that are too small
            for (int i = areas.Count() - 1; i >= 0; i--)
            {
                var area = areas[i];
                if (filter == null || filter(area))
                {
                    SetArea(map, area, newTileID);
                    //areas.Remove(area);
                } 
            }

            return map;
        }


        protected virtual List<LakeData> GetLakeTypes(Temp temp)
        {
            var list = new List<LakeData>(2);
            switch (temp)
            {
                default:
                    list.Add(new LakeData(BlockID.Water, 45));
                    break;
                case Temp.Freezing:
                    list.Add(new LakeData(BlockID.Ice, 45));
                    break;
                case Temp.Glacial:
                    list.Add(new LakeData(BlockID.Ice, 45));
                    break;
                case Temp.Hot:
                    list.Add(new LakeData(BlockID.Water, 35));
                    list.Add(new LakeData(BlockID.Lava, 35));
                    list.Add(new LakeData(BlockID.Sulphur, 25));
                    break;
                case Temp.Scorching:
                    list.Add(new LakeData(BlockID.Lava, 45));
                    list.Add(new LakeData(BlockID.Sulphur, 35));
                    break;
            }
            return list;
        }


        protected void SetArea(int[,] map, Area area, int tileID)
        {
            foreach (var point in area.Nodes)
                map[point.X, point.Y] = tileID;
        }

        protected int[,] OverlapMaps(int[,] map1, int[,] map2, int overlapID)
        {
            //adds floor tiles from map2 to map1 as water.
            var tempMap = new int[MapWidth, MapHeight];

            int type;
            for (int x, y = 0; y < MapHeight; y++)
            {
                for (x = 0; x < MapWidth; x++)
                {
                    if (map2[x, y] == BlockID.Floor && CanLakeOverlap(map1[x, y]))
                    {
                        type = overlapID;
                    }
                    else
                    {
                        type = map1[x, y];
                    }

                    /*
                    //type = map1[x, y] == TileID.Wall ? TileID.Wall : map2[x, y] == TileID.Floor ? TileID.Water : TileID.Floor;
                    type = map1[x, y] == TileID.Floor ? TileID.Floor : map2[x, y] == TileID.Floor ? TileID.Water : TileID.Wall;
                    //type = map2[x, y] == TileID.Floor ? TileID.Water : map1[x, y];
                    */

                    tempMap[x, y] = type;
                }
            }

            return tempMap;
        }

        protected virtual bool CanLakeOverlap(int type)
        {
            switch (type)
            {
                default:
                    return false;

                case BlockID.Wall:
                //case BlockID.Floor:
                    return true;
            }
        }


        public List<Area> IdentifyAreas(int[,] map, int tileID)
        {
            //create a temp map to be able to differentiate
            var tempMap = CopyMap(map);
            var list = new List<Area>(10);

            //loop through map and find areas of requested type.
            for (int x, y = 1; y < MapHeight - 1; y++)
            {
                for (x = 1; x < MapWidth - 1; x++)
                {
                    if (tempMap[x, y] == tileID)
                    {
                        Area connected = FloodFillFromPoint(tempMap, x, y);
                        list.Add(connected);
                    }
                }
            }

            return list;
        }

        private Area FloodFillFromPoint(int[,] map, int x, int y)
        {
            int type = map[x, y];
            Stack<Vector> undone = new Stack<Vector>();
            List<Vector> connectedPoints = new List<Vector>();

            undone.Push(new Vector(x, y));

            while (undone.Count() > 0)
            {
                Vector point = undone.Pop();

                // Add point to connected area list - ERROR: for now check whether existent: else you get duplicate tiles...
                if (!connectedPoints.Contains(point))
                    connectedPoints.Add(point);

                // IMPORTANT: repaint points to avoid infinite loop!
                map[x, y] = -999;

                x = point.X;
                y = point.Y;

                // Check surrounding tiles
                if (x > 0 && map[x - 1, y] == type)
                    undone.Push(new Vector(x - 1, y));
                if (x < MapWidth - 1 && map[x + 1, y] == type)
                    undone.Push(new Vector(x + 1, y));
                if (y > 0 && map[x, y - 1] == type)
                    undone.Push(new Vector(x, y - 1));
                if (y < MapHeight - 1 && map[x, y + 1] == type)
                    undone.Push(new Vector(x, y + 1));
            }

            Cavern cavern = new Cavern(Level, connectedPoints);
            return cavern;
        }


        /// <summary>
        /// Smoothifies the supplied map by averaging each tile on it's surroudings
        /// </summary>
        public static int[,] SmoothifyMap(int[,] map)
        {
            int w = map.GetLength(0), h = map.GetLength(1);
            
            // Create new temporary map
            var tempMap = new int[w, h];
            tempMap.Populate(BlockID.Wall);

            //pass over every tile on the map
            for (int x, y = 1; y < h - 1; y++)
            {
                for (x = 1; x < w - 1; x++)
                {
                    tempMap[x, y] = SmoothifyTile(map, x, y);
                }
            }

            return tempMap;

        }


        /// <summary>
        /// This method returns a new tile type based on its surroundings
        /// </summary>
        protected static int SmoothifyTile(int[,] map, int x, int y)
        {
            int amtOfWalls = CountSurroundingWalls(map, x, y);

            if (map[x, y] == BlockID.Wall)
            {
                return amtOfWalls < WallDeath ? BlockID.Floor : BlockID.Wall;
            }
            else
            {
                return amtOfWalls >= FloorDeath ? BlockID.Wall : BlockID.Floor;
            }
        }

        protected static int CountSurroundingWalls(int[,] map, int x, int y)
        {
            int amtOfWalls = 0;

            // Setup area to check
            int detectSize, x1, y1, x2, y2;
            detectSize = 1;
            x1 = x - detectSize;
            y1 = y - detectSize;
            x2 = x + detectSize;
            y2 = y + detectSize;

            //check tiles in area
            int i, j;
            for (j = y1; j <= y2; j++)
            {
                for (i = x1; i <= x2; i++)
                {
                    if (!(i == x && j == y))
                    {
                        if (IsWall(map, i, j))
                        {
                            amtOfWalls++;
                        }
                    }
                }
            }

            return amtOfWalls;
        }

        protected static bool IsWall(int[,] map, int x, int y)
        {
            // Either out-of-bounds or the value equals true
            return IsOutOfBounds(map, x, y) || map[x, y] == BlockID.Wall;
        }

        public static bool IsOutOfBounds(int[,] map, int x, int y)
        {
            return x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1);
        }

        public static int[,] RandomFillMap(int[,] map, int percentFloor, Random random)
        {
            int w = map.GetLength(0), h = map.GetLength(1);
            
            int x, y;
            for (y = 0; y < h; y++)
            {
                for (x = 0; x < w; x++)
                {
                    // Create outer wall border
                    if (x == 0 || y == 0 || x == w - 1 || y == h - 1)
                    {
                        map[x, y] = 1;
                    }

                    // Set as random tile
                    else
                    {
                        map[x, y] = RandomPercent(percentFloor, random);
                    }
                }
            }

            return map;
        }

        protected static int RandomPercent(int percent, Random random)
        {
            if (percent >= random.Next(1, 101))
            {
                return BlockID.Floor;
            }
            else return BlockID.Wall;
        }


        /// <summary>
        /// Creates a copy of the supplied map
        /// </summary>
        public int[,] CopyMap(int[,] map)
        {
            int[,] copy = new int[MapWidth, MapHeight];

            for (int x, y = 0; y < MapHeight; y++)
                for (x = 0; x < MapWidth; x++)
                    copy[x, y] = map[x, y];

            return copy;
        }

        //private bool IsSuitable(Vector point, int[,] map)
        //{
        //    This method returns true if area - 2 to + 2 around point has no wall / water tiles

        //    int x1 = Math.Max(0, point.X - 1);
        //    int y1 = Math.Max(0, point.Y - 1);
        //    int x2 = Math.Min(MapWidth - 1, point.X + 1);
        //    int y2 = Math.Min(MapHeight - 1, point.Y + 1);

        //    for (int i, j = y1; j <= y2; j++)
        //    {
        //        for (i = x1; i <= x2; i++)
        //        {
        //            int type = map[i, j];
        //            if (type != TileID.Floor)
        //                return false;
        //        }
        //    }

        //    return true;
        //}


        /// <summary>
        /// This method connects all areas and makes all areas accessible with with paths
        /// </summary>
        protected void ConnectNonConnectedRooms(Tile[,] map, List<Area> areas)
        {
            var areas2 = areas.CreateCopy();
            // Find all room-combinations that arent connected
            foreach (var area1 in areas2.Where(r => r is Room))
            {
                foreach (var area2 in areas2.Where(r => r is Room))
                {
                    if (area1 != area2 && !area1.Connections.Any(c => c.GetOther(area1) == area2))
                    {
                        var cp = CreateConnectionPotential(area1, area2);
                        areas.Remove(area1);
                        ConnectOverConnectionPotential(cp, map, areas, new List<Area>() { area1 });
                    }
                }
            }
        }

        /// <summary>
        /// This method connects all areas and makes all areas accessible with with paths
        /// </summary>
        protected void ConnectAreas(Tile[,] map, List<Area> areas)
        {
            // Create list of connected areas
            var remainingAreas = areas.CreateCopy();
            var areaMap = new List<Area>();
            
            // Add first entry
            var startingArea = remainingAreas[Random.Next(remainingAreas.Count)];
            areaMap.Add(startingArea);
            remainingAreas.Remove(startingArea);


            // Combine all areas until one area map is left
            while (remainingAreas.Count() > 0)
            {
                // Find closest area to areaMap by calculating closest edge-point-distance
                ConnectNearestArea(map, ref areaMap, ref remainingAreas);
            }
        }

        /// <summary>
        /// Connects the nearest area in the remaining areas to the areamap of connected areas.
        /// There's a chance to form a loop connection.
        /// </summary>
        /// <param name="map">The tilemap.</param>
        /// <param name="areaMap">The list of already connected areas.</param>
        /// <param name="remainingAreas">The list of areas not yet connected.</param>
        private void ConnectNearestArea(Tile[,] map, ref List<Area> areaMap, ref List<Area> remainingAreas)
        {
            // Compile list of connection potentials
            var connections = ListConnectionPotentials(areaMap, remainingAreas);

            // Find shortest connection
            var connection = FindShortest(connections);

            // Decide whether a loop could be made at all
            bool canLoop = areaMap.Count() > 1;

            // Connect areas
            ConnectOverConnectionPotential(connection, map, areaMap, remainingAreas);
            connections.Remove(connection);

            // if its a room and there are bordering rooms that aren't connected:
            if (canLoop && connection.NewArea is Room)
            {
                // Find connections that come from an existing room
                var neighbouringRooms = connections.Where(c => c.NewArea == connection.NewArea && c.BaseArea is Room && c.Distance == 1).ToList();

                foreach (var n in neighbouringRooms)
                {
                    ConnectOverConnectionPotential(n, map, areaMap, remainingAreas);
                }

                //Level.Server.User.ShowMessage("<yellow>Inter-room connections: " + neighbouringRooms.Count);
            }

            // 25% Chance to add a loop connection
            if (canLoop && Random.Next(100) < 25)
            {
                // Compile list of alternative connections:
                connections = connections.Where(c => c.NewArea == connection.NewArea).ToList();

                // Find nearest other connection
                var alternativeConnection = FindShortest(connections);

                // If new path isnt longer than 2x the current path:
                if (alternativeConnection.Distance <= (connection.Distance + 3) * 2)
                {
                    ConnectOverConnectionPotential(alternativeConnection, map, areaMap, remainingAreas);
                }
            }

        }

        private List<ConnectionPotential> ListConnectionPotentials(List<Area> areaMap, List<Area> remainingAreas)
        {
            var connections = new List<ConnectionPotential>();

            // Compile list of potential connections by temporarily connecting all new Areas to all base Areas:
            foreach (var area2 in remainingAreas)
            {
                foreach (var area1 in areaMap)
                {
                    connections.Add(CreateConnectionPotential(area1, area2));
                }
            }

            return connections;
        }

        private ConnectionPotential CreateConnectionPotential(Area area1, Area area2)
        {
            // Connect areas directly (without nodes):
            if (area1.IsTouching(area2))
            {
                var node = area1.GetTouchingNodes(area2).PickRandom(Random);
                return new ConnectionPotentialTouching(area1, area2, node, node);
            }
            // Connect areas by pathway:
            else
            {
                int distance, lowestDistance = 100;
                Vector node1 = Vector.Zero, node2 = Vector.Zero;

                // Calculate shortest possible distance between new and base area
                foreach (var pointB in area2.EdgeNodes)
                {
                    foreach (var pointA in area1.EdgeNodes)
                    {
                        distance = (pointB - pointA).OrthogonalLength();

                        if (distance < lowestDistance)
                        {
                            lowestDistance = distance;
                            node1 = pointA;
                            node2 = pointB;
                        }
                    }
                }

                return new ConnectionPotential(area1, area2, node1, node2);
            }
        }

        private static ConnectionPotential FindShortest(List<ConnectionPotential> list)
        {
            ConnectionPotential shortest = null;
            foreach (var item in list)
            {
                if (item is ConnectionPotentialTouching)
                    return item;

                if (shortest == null)
                    shortest = item;
                else if (item.Distance < shortest.Distance)
                    shortest = item;
            }

            return shortest;
        }

        /// <summary>
        /// Connects the areas in the described connectionPotential.
        /// Places the new area of the connectionPotential from the remainingAreas list to the areaMap list.
        /// Connects the areas either directly via a an actual connection or indirectly with a path in between.
        /// Can fail when the pathway crosses another area.
        /// </summary>
        /// <param name="cp">The connectionpotential containing the connection type and the two areas.</param>
        /// <param name="map">The tilemap.</param>
        /// <param name="areaMap">The list of connected areas.</param>
        /// <param name="remainingAreas">The list of remaining areas.</param>
        /// <returns>Whether the connection could be made.</returns>
        private bool ConnectOverConnectionPotential(ConnectionPotential cp, Tile[,] map, List<Area> areaMap, List<Area> remainingAreas)
        {
            // Connect directly:
            if (cp is ConnectionPotentialTouching)
            {
                cp.BaseArea.ConnectWith(cp.NewArea, cp.BasePoint);
            }
            // Connection over pathway:
            else
            {
                var pathway = MakePathway(map, cp.BasePoint, cp.NewPoint);
                
                foreach (var area in areaMap.Concat(remainingAreas))
                    if (pathway.Overlaps(area))
                        return false;

                Areas.Add(pathway);
                areaMap.Add(pathway);

                // Connect a1 to path to a2
                cp.BaseArea.ConnectWith(pathway, cp.BasePoint);
                pathway.ConnectWith(cp.NewArea, cp.NewPoint);
            }

            // Place new area on the 'done' list
            remainingAreas.Remove(cp.NewArea);
            areaMap.Add(cp.NewArea);
            return true;
        }


        /// <summary>
        /// Creates a pathway from supllied start to end on the supplied map
        /// </summary>
        /// <param name="map">The map to draw on.</param>
        /// <param name="start">The starting node of the pathway (inclusive).</param>
        /// <param name="end">The ending node of the pathway (inclusive).</param>
        /// <returns></returns>
        private Pathway MakePathway(Tile[,] map, Vector start, Vector end)
        {
            //return new Pathway(new List<Vector>());
            // Create list of nodes
            var pathwayNodes = new List<Vector>();
            var node = start;


            // Walk horizontally:
            while (node.X != end.X)
            {
                pathwayNodes.Add(new Vector(node.X, node.Y));
                node.X += Math.Sign(end.X - node.X);
            }
            int changeDirIndex = pathwayNodes.Count;

            // Walk vertically:
            while (node.Y != end.Y)
            {
                pathwayNodes.Add(new Vector(node.X, node.Y));
                node.Y += Math.Sign(end.Y - node.Y);
            }
            pathwayNodes.Add(end);


            // Add patway to map:
            int groundID, pathID, i = 0;
            foreach (var point in pathwayNodes)
            {
                // Base the pathing tile and layer on the current ground id:
                groundID = map[point.X, point.Y].Ground.ID;
                pathID = GetPathType(groundID);

                // If the path id is just a floor, change the ground id to "Floor", otherwise change the object id:
                if (pathID == BlockID.Floor)
                {
                    //groundID = pathID;
                    map[point.X, point.Y].Ground.ID = pathID;
                }
                else
                {
                    map[point.X, point.Y].Object.ID = pathID;

                    // Select the Dock tile orientation
                    if (pathID == BlockID.Dock)
                        map[point.X, point.Y].Object.Value = i < changeDirIndex ? 0 : 1;
                }


                // Smoothify surrounding tiles of edited tile
                /*
                //identify surrounding tiles
                var surroundingTiles = GetSurroundingTiles(point.X, point.Y, TileMap);
                foreach (var surrounding in surroundingTiles)
                {
                    var type = TileMap[surrounding.X, surrounding.Y];

                    //get the surrounding's surrounding tiles
                    var surroundingTiles2 = GetSurroundingTiles(surrounding.X, surrounding.Y, TileMap);

                    if (type == TileID.Wall)
                    {
                        var amtOfWalls = surroundingTiles2.Count(p => TileMap[p.X, p.Y] == TileID.Wall);
                        if (amtOfWalls <= 1) TileMap[surrounding.X, surrounding.Y] = 0;
                    }
                    else if (type == TileID.Water)
                    {
                        var amtOfWalls = surroundingTiles2.Count(p => TileMap[p.X, p.Y] == 1 || TileMap[p.X, p.Y] == 2 || TileMap[p.X, p.Y] == 3);
                        if (amtOfWalls <= 1) TileMap[surrounding.X, surrounding.Y] = 0;
                    }
                }
                */

                i++;
            }

            var pathway = new Pathway(Level, pathwayNodes);
            pathway.IdentifyEdgePoints();

            return pathway;
        }

        protected int GetPathType(int oldID)
        {
            switch (oldID)
            {
                default:
                    return BlockID.Floor;
                case BlockID.Water:
                case BlockID.Lava:
                case BlockID.Sulphur:
                case BlockID.Poison:
                case BlockID.Chasm:
                case BlockID.Ice:
                    return BlockID.Dock;
            }
        }


        /// <summary>
        /// Allow when the room can be built on at least 80% of the existing here.
        /// </summary>
        private bool CanRoomBeBuilt(int[,] map, Room room)
        {
            int amountOfAllowedTiles = 0;

            foreach (var node in room.EdgeNodes)
            {
                if (map[node.X, node.Y] == ROOM_ID)
                    return false;
            }

            foreach (var node in room.Nodes)
            {
                if (map[node.X, node.Y] == ROOM_ID)
                    return false;

                if (CanRoomBeBuiltOnType(map[node.X, node.Y]))
                    amountOfAllowedTiles++;
            }

            // Return true if over 80% of tiles are allowed
            return amountOfAllowedTiles * 100 / room.Nodes.Count >= 80;
        }

        protected virtual bool CanRoomBeBuiltOnType(int type)
        {
            switch (type)
            {
                default:
                    return false;
                case BlockID.Wall:
                case BlockID.Bricks:
                    return true;
            }
        }


        /// <summary>
        /// Expands the map by a border all around.
        /// NOTE: Areas should be re-identified as they will not get updated!
        /// Updates the global map width and height.
        /// </summary>
        /// <param name="amt">The border width.</param>
        protected int[,] ExpandMapBorder(int[,] map, int amt)
        {
            int newWidth, newHeight;
            newWidth = MapWidth + 2 * amt;
            newHeight = MapHeight + 2 * amt;

            var newMap = new int[newWidth, newHeight];
            newMap.Populate(BlockID.Wall);
            for (int i, j = 0; j < MapHeight; j++)
            {
                for (i = 0; i < MapWidth; i++)
                {
                    newMap[amt + i, amt + j] = map[i, j];
                }
            }

            MapWidth = newWidth;
            MapHeight = newHeight;

            return newMap;
        }
    }
}

