using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public class Populator
    {
        protected readonly Random Random;
        public Level Level { get; private set; }

        public readonly int
            MapWidth = 48,
            MapHeight = 48;

        public Tile[,] TileMap => Level.TileMap;

        protected List<Area> Areas;
        //private List<Area> Lakes;

        protected IEnumerable<Area> GetNonPathwayAreas() => Areas.Where(a => !(a is Pathway));

        protected Area Start, End;
        protected Vector StartPosition, EndPosition;

        protected readonly List<string> KeyTypes = new List<string>() { "gold", "silver", "iron", "steel", "nickel", "brass"};

        protected readonly List<Spawner> Spawners = new List<Spawner>();
        public void AddSpawner(Spawner s) => Spawners.Add(s);


        // Difficulty points - used to determine how many and what type of enemies can be spawned.
        protected virtual int CalculateDifficultyPoints(int depth) => (int)(Math.Pow(1.015d, depth) * 30);


        public Populator(Level level)
        {
            Level = level;
            Random = level.Random;
            MapWidth = level.MapWidth;
            MapHeight = level.MapHeight;
            Areas = level.Areas;
        }

        public void PopulateLevel()
        {
            // if Areas is to be shuffled, do it here, before creating references.

            // Set level data properties
            Level.AreaReferences = ReferenceAreas();
            Level.DifficultyPoints = CalculateDifficultyPoints(Level.Depth);

            // Check tile overlap!
            foreach (var area in Areas)
            {
                foreach (var node in area.Nodes)
                {
                    var retrieved = Level.AreaReferences[node.X, node.Y];
                    var id = area.ID;
                    if (retrieved != id)
                        Level.Server.ThrowDebug("Area-node incongruity: " + node + " (id " + id + " vs " + retrieved + ") " + area);
                }
            }

            // Basic area init:
            foreach (var area in Areas)
            {
                area.ListFreeNodes();
                area.FreeNodes.Shuffle(Random);
                area.CalculateRemoteNode();
                area.ImportantTileSets.Add(new List<Vector>() { area.RemoteNode });
            }

            // Assign Start and End locations.
            AssignStartAndEnd();
            Start.IsStart = true;
            End.IsEnd = true;
            Start.EventLocations.Add(StartPosition);
            Level.StartPosition = StartPosition;
            Level.EndPosition = EndPosition;

            // Populate the dungeon!
            PopulateAreas();

            // Remove empty areas to be sure:
            int c = Areas.Count;
            Areas = Areas.Where(a => a.Size > 0).ToList();
            if (c - Areas.Count > 0)
                Level.Server.User.ThrowError("Removed " + (c - Areas.Count) + " empty areas.");

            //foreach (var area in Areas.Where(a => !(a is Pathway)))
            //    Level.Server.ThrowDebug(area.ToString());

            if (Level.LevelManager.Server.GameClient.IsDevMode)
                DrawCrucialNodes();
        }

        protected virtual void PopulateAreas()
        {
            // Populate the dungeon
            PaintDungeon();
            AssignRoomGenerators(Level.Depth, Level.Temp);
            CreateDungeonStructure();

            Spawners.Shuffle();
            foreach (var s in Spawners)
                s.Spawn();
        }

        protected virtual void AssignStartAndEnd()
        {
            // Compile candidates: remote to remote node distance:
            var areas = GetNonPathwayAreas();
            var candidates = new List<Tuple<int, Area, Area>>();
            foreach (var a1 in areas) {
                foreach (var a2 in areas) {
                    var dist = (a1.RemoteNode - a2.RemoteNode).ChebyshevLength();
                    candidates.Add(Tuple.Create(dist, a1, a2));
                }
            }

            // Find largest distance candidate
            var sorted = candidates.OrderByDescending(c => c.Item1);
            foreach (var c in sorted)
            {
                Start = c.Item2;
                End = c.Item3;
                
                var stairsUp = new StairsUp(0, 0);
                var stairsDown = new StairsDown(0, 0);
                if (FindStairsLocation(Start, stairsUp)) //Start.FindInstanceSpawn(stairsUp, Start.RemoteNode, 5, true, true)
                {
                    // Added
                    foreach (var node in Vector.Dirs8)
                        Start.FreeNodes.Remove(node + stairsUp.Position);
                    
                    if (End.FindInstanceSpawn(stairsDown, End.RemoteNode, 5, true, true))
                    {
                        StartPosition = stairsUp.Position;
                        EndPosition = stairsDown.Position;
                        return;
                    }
                    else
                    {
                        Start.RemoveInstance(stairsUp);
                        // Added
                        foreach (var node in Vector.Dirs8)
                            Start.FreeNodes.Add(node + stairsUp.Position);
                    }  
                }
            }
        }

        protected bool FindStairsLocation(Area area, Instance stairs)
        {
            var suitables = area.FreeNodes.Where(fn => area.FreeNodes.Count(empty => (fn - empty).ChebyshevLength() == 1) == 8).ToList();
            suitables.Sort((a, b) => (a - area.RemoteNode).ChebyshevLength() - (b - area.RemoteNode).ChebyshevLength());
            return suitables.Any(s => 
                {
                    stairs.Position = s;
                    return area.TrySpawnInstance(stairs, true);
                });
        }



        private void DrawCrucialNodes()
        {
            foreach (var area in Areas) {
                area.CheckTilesObstruct(null, true);

                foreach (var node in area.CrucialNodes.Distinct()) {
                    if (!AccessibilityChecker.IsWalkableTile(Level, node)) {
                        Level.Server.ThrowError("Obstructing node: " + node + Level.GetTile(node));
                        Level.MarkTile(node, 5);
                    }
                    else
                        Level.MarkTile(node, 1);
                }

                // Draw connections
                foreach (var connection in area.Connections) {
                    foreach (var node in connection.Nodes) {
                        var id = AccessibilityChecker.IsWalkableTile(Level, node)
                            ? 2
                            : 5;
                        Level.MarkTile(node, id);
                    }
                }
            }
        }

        protected virtual void PaintDungeon()
        {
            if ((int)Level.Temp >= (int)Temp.Cold && (int)Level.Temp <= (int)Temp.Warm)
                Paint(new BlobBrush(BlockID.Mud, true, false, true, 7), 5);

            Paint(new ScatterBrush(BlockID.Rock, false, true, true, 3), 7);
            Paint(new SpreadBrush(BlockID.Mushroom, false, true, true, 0), 3);
            
            if (Level.Depth > 15)
                Paint(new ScatterBrush(BlockID.Crystal, false, true, true, 3), 4);

            if (Level.Temp == Temp.Freezing || Level.Temp == Temp.Glacial)
                Paint(new ScatterBrush(BlockID.Icicle, false, false, true, 3), 7);
        }

        protected void Paint(IBrush brush, int repetitions)
        {
            int amt = 0;
            for (int i = 0; i < repetitions; i++)
            {
                var start = new Vector(Random.Next(MapWidth), Random.Next(MapHeight));
                if (brush.Paint(Level, start, Random) > 0)
                    amt++;
            }
        }


        /// <summary>
        /// This method creates a grid with ints as reference id to every area. -1 is default.
        /// </summary>
        protected int[,] ReferenceAreas()
        {
            var map = new int[MapWidth, MapHeight];
            map.Populate(-1);

            for (int i = 0; i < Areas.Count; i++)
            {
                var area = Areas[i];
                area.ID = i;

                foreach (var node in area.Nodes) {
                    map[node.X, node.Y] = i;
                }
            }

            return map;
        }


        protected virtual void CreateDungeonStructure()
        {
            // Copy all areas and shuffle them
            var areas = Areas.CreateCopy();
            areas.Shuffle();
            KeyTypes.Shuffle();

            // Find a starting area to spawn in
            //Start = FindStart(areas);
            //End = FindEnd(areas);

            // Start iterating
            var visitedRooms = new List<Area>(Areas.Count);
            var keyPotentials = new List<Area>();
            var undoneRooms = new Stack<Area>();
            undoneRooms.Push(Start);
            while (undoneRooms.Count() > 0)
            {
                // Retrieve current & mark it visited
                var current = undoneRooms.Pop();

                // ARE these TWO NECESSARY?? Latter is called twice!
                if (current == null)
                    continue;
                if (visitedRooms.Contains(current))
                    continue;

                visitedRooms.Add(current);
                
                // Add new connecting rooms to the iterator
                foreach (var connection in current.Connections)
                {
                    var other = connection.GetOther(current);
                    if (!visitedRooms.Contains(other))
                    {
                        undoneRooms.Push(other);
                    }

                    // Add doors (between rooms & pathways only, some are of them are locked)
                    if (!connection.HasDoor && current is Room room && connection.GetOther(room) is Pathway path)
                    {
                        // Chance 1/2 of having a door
                        if (room.IsEnclosed || Random.Next(2) == 0)
                        {
                            CreateDoor(current, connection, keyPotentials);
                            path.FreeNodes.Remove(connection.GetNode());
                        }
                    }
                }

                if (!(current is Pathway))
                    keyPotentials.Add(current); // This is placed here: doors can be created backwards!

                // Populate current room:
                current.Populate(Random, this);
            }

            //Level.StartPosition = StartPosition;
            //Level.EndPosition = EndPosition;
        }

        /// <summary>
        /// Creates a door at the specified connection.
        /// If both a keyPotential & a keyType is present, a locked door might spawn.
        /// Then this keyType is removed, and 
        /// </summary>
        /// <param name="baseArea"></param>
        /// <param name="connection"></param>
        /// <param name="keyPotentials"></param>
        protected void CreateDoor(Area baseArea, Connection connection, List<Area> keyPotentials)
        {
            // Return if the connection's tile is occupied:
            if (Level.ActiveInstances.Any(i => i.GetTiles().Contains(connection.GetNode())))
                return;
            
            Door door = null;

            // Chance to be a locked door, if there are keys left and there's a place to put them
            if (Random.Next(6) == 0 && keyPotentials.Count() > 0 && KeyTypes.Count > 0)
            {
                var lockType = KeyTypes.PickRandom(Random);
                var key = Item.Create(lockType + "_key");

                // Pick a key area - anything but the current room if it is a pathway
                //var keyArea = keyPotentials.Where(a => !((a == connection.A1 || a == connection.A2) && a is Pathway)).ToList().PickRandom(Random);
                var keyArea = keyPotentials.PickRandom(Random); //.Where(a => !(a is Pathway)).ToList()

                if (keyArea.FindInstanceSpawn(new GroundItem(0, 0, key), true))
                {
                    var doorPos = connection.GetNode();
                    door = new LockedDoor(doorPos.X, doorPos.Y, lockType);

                    // Remove the keyType & keyPotential from the lists
                    KeyTypes.Remove(lockType);
                    keyPotentials.Remove(keyArea);
                }
            }
            
            // If door is still null, there is no locked door, or it failed to spawn
            if (door == null)
            {
                door = new Door(0, 0);
            }

            // Create the door instance
            var node = connection.GetNode();
            door.Position = node;
            Level.CreateInstance(door);

            connection.HasDoor = true;
        }

        /// <summary>
        /// This method attempts to find a spawn area and places the stairs up.
        /// </summary>
        protected Area FindStart(List<Area> areas)
        {
            var stairs = new StairsUp(0, 0);
            foreach (var area in areas)
            {
                if (!(area is Pathway))
                {
                    if (area.FindInstanceSpawn(stairs, true))
                    {
                        StartPosition = stairs.Position;
                        return area;
                    }
                }
            }

            return null;
        }

        protected Area FindEnd(List<Area> areas)
        {
            var stairs = new StairsDown(0, 0);
            foreach (var area in areas)
            {
                if (!(area is Pathway) && area != Start)
                {
                    if (area.FindInstanceSpawn(stairs, true))
                    {
                        EndPosition = stairs.Position;
                        return area;
                    }
                }
            }

            return null;
        }

        protected void AssignRoomGenerators(int depth, Temp temp)
        {
            // Track current
            var currentTypes = new List<Type>();

            // Assign
            var areas = GetNonPathwayAreas().ToList();
            areas.Sort((a, b) => 
                {
                    var c1 = a.Connections.Count <= 1 ? 10 : a.Connections.Count;
                    var c2 = b.Connections.Count <= 1 ? 10 : b.Connections.Count;
                    return c2 - c1;
                });

            foreach (var area in areas)
            {
                var type = area == Start && depth > 1
                    ? typeof(AreaGeneration.AreaShrine)
                    : area == End && area.Size < 40
                    ? null
                    : AreaGeneration.AreaData.PickPrimaryAreaType(area, depth, temp, currentTypes, Random);
                if (type == null) continue;
                area.Generator = (AreaGeneration.AreaGenerator)Activator.CreateInstance(type);
                currentTypes.Add(type);
            }
        }

        public static Instance CreateInstanceFromType(Type type)
        {
            if (!typeof(Instance).IsAssignableFrom(type))
                return null;

            else return (Instance)Activator.CreateInstance(type, 0, 0);
        }
    }
}
