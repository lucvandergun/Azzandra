using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.Generation
{
    public abstract class Area
    {
        // === Properties === \\
        public Level Level;

        public List<Vector> Nodes = new List<Vector>();     // All nodes beloning to this area.
        public List<Vector> EdgeNodes = new List<Vector>(); // All nodes just outside of this area (bordering a node in 'Nodes').
        public List<Vector> FreeNodes = new List<Vector>(); // All nodes yet unoccupied; is a subset of 'Nodes'. Should be/is updated with new spawns.
        public List<List<Vector>> ImportantTileSets = new List<List<Vector>>(); // Sets of sets of nodes that must be reachable at all times
        public List<Connection> Connections = new List<Connection>();           // The set of connections this area has with other areas.

        public List<Vector> CrucialNodes = new List<Vector>();      // Set of nodes that show an unobstructed path between all important tilesets and connections. Used for debugging.
        public List<Vector> EventLocations = new List<Vector>();    // Set of locations to help with proximity constraints of potential new events.
        public Vector RemoteNode;       // The node in 'Nodes', farthest from all connections of the area.

        public int ID = -1;         // The unique area identifier.

        public AreaGeneration.AreaGenerator Generator;     // The room generator set to be associated with this area.
        public bool IsStart { get; set; } = false;
        public bool IsEnd { get; set; } = false;


        // === Getters === \\
        public int Size => Nodes.Count;
        public override string ToString()
        {
            var zero = Size > 0 ? Nodes[0] + "" : "none";
            return GetType().Name + " [size: " + Size + ", first: " + zero + ", type: " + (Generator?.GetType().Name ?? "Empty") + "]";
        }

        /// <summary>
        /// Compiles the subset of 'FreeNodes' of which all 8 surrounding nodes are also in 'FreeNodes'.
        /// </summary>
        public List<Vector> GetCenterFreeNodes()
        {
            return FreeNodes.Where(f => Vector.Dirs8.TrueForAll(d => FreeNodes.Exists(n => f + d == n))).ToList();
        }



        // === One-Off Setters === \\
        public Area(Level world, List<Vector> listOfPoints)
        {
            Level = world;
            Nodes = listOfPoints;
        }

        public void ListFreeNodes()
        {
            FreeNodes = Nodes.CreateCopy();
        }

        public void CalculateRemoteNode()
        {
            var avgEntry = new Vector();
            foreach (var c in Connections) avgEntry += c.GetNode();
            avgEntry /= 3;

            int longestDist = -1;
            foreach (var n in Nodes) {
                int dist = (avgEntry - n).ChebyshevLength();
                if (dist > longestDist) {
                    RemoteNode = n;
                    longestDist = dist;
                }
            }
        }

        /// <summary>
        /// Create a list of nodes located just outside the the nodes of the area.
        /// </summary>
        public virtual void IdentifyEdgePoints()
        {
            EdgeNodes.Clear();

            // Iterate through all points of this area
            foreach (var point in Nodes)
            {
                //// Check each ajacent square on the map for a non-floor tile
                //Vector[] adjacent = { 
                //    new Vector(point.X - 1, point.Y), 
                //    new Vector(point.X + 1, point.Y), 
                //    new Vector(point.X, point.Y - 1), 
                //    new Vector(point.X, point.Y + 1) };

                //foreach (var adj in adjacent)
                //{
                //    if (map[adj.X, adj.Y] != TileID.Floor)
                //    {
                //        // The point is at the area border -> add to list
                //        ListOfEdgePoints.Add(adj);
                //        break;
                //    }
                //}

                var adjacent = new List<Vector>(4);
                if (point.X > 0) adjacent.Add(new Vector(point.X - 1, point.Y));
                if (point.X < 40 - 1) adjacent.Add(new Vector(point.X + 1, point.Y));
                if (point.Y > 0) adjacent.Add(new Vector(point.X, point.Y - 1));
                if (point.Y < 40 - 1) adjacent.Add(new Vector(point.X, point.Y + 1));

                foreach (var adj in adjacent)
                {
                    if (!Nodes.Contains(adj) && !EdgeNodes.Contains(adj))
                    {
                        // The point is at the area border -> add to list
                        EdgeNodes.Add(adj);
                    }
                }
            }
        }



        // === Generation === \\

        /// <summary>
        /// Connects this area to another over a connecting node.
        /// </summary>
        public void ConnectWith(Area otherArea, Vector connectingNode) => ConnectWith(otherArea, new List<Vector> { connectingNode });
        public void ConnectWith(Area otherArea, List<Vector> connectingNodes)
        {
            if (otherArea == null)
                return;

            var c = new Connection(this, otherArea, connectingNodes);

            Connections.Add(c);
            otherArea.Connections.Add(c);
        }

        /// <summary>
        /// Checks whether any node in this area coincides with a node in the other area.
        /// </summary>
        public bool Overlaps(Area other)
        {
            return Nodes.Intersect(other.Nodes).Count() > 0;
        }

        /// <summary>
        /// Checks whether this area is touching another area.
        /// </summary>
        public virtual bool IsTouching(Area area)
        {
            var touching =
                (EdgeNodes.Intersect(area.Nodes)).Count() > 0 ||
                (Nodes.Intersect(area.EdgeNodes)).Count() > 0;

            return touching;
        }

        /// <summary>
        /// Computes the set of nodes of this area that touch nodes of the other area.
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public virtual List<Vector> GetTouchingNodes(Area area)
        {
            var a1 = (EdgeNodes.Intersect(area.Nodes));
            if (a1.Count() > 0) return a1.ToList();

            var a2 = (Nodes.Intersect(area.EdgeNodes));
            return a2.ToList();
        }



        // === Population: Tile & Instance Spawning === \\
        public virtual bool IsReachable(List<Vector> points1, List<Vector> points2, List<Vector> obstructingTiles, bool draw = false)
        {
            if (points1 == null || points2 == null)
                return true;

            //foreach (var p1 in points1) {
            //    foreach (var p2 in points2) {
            //        if (AccessibilityChecker.IsAccessible(Level, this, p1, p2, obstructingTiles, draw))
            //        {
            //            return true;
            //        }
            //    }
            //}

            return AccessibilityChecker.IsAccessible(Level, this, points1, points2, obstructingTiles, draw);

            //return false;
        }


        /// <summary>
        /// Pass-on function.
        /// </summary>
        public virtual bool CheckInstanceObstructs(Instance inst)
        {
            // Skip checking if instance isn't solid or an entity
            if (!inst.IsSolid() || inst is Entity)
                return false;

            var obstructingTiles = inst.GetTiles().ToList();

            return CheckTilesObstruct(obstructingTiles);
        }

        /// <summary>
        /// Pass-on function.
        /// </summary>
        public virtual bool CheckTileObstructs(Vector pos, int blockID)
        {
            if (BlockID.GetData(blockID).IsWalkable)
                return false;

            return CheckTilesObstruct(new List<Vector>() { pos });
        }

        /// <summary>
        /// This function checks whether all connecting nodes are accessible interchangebly
        /// </summary>
        /// <param name="obstructingTiles">The set of tiles to check for their obstruction.</param>
        /// <param name="draw">Whether to accumulate the resulting nodes (are added to 'CrucialNodes').</param>
        /// <returns>Whether the totality of given tiles obstructs the inner area paths.</returns>
        public virtual bool CheckTilesObstruct(List<Vector> obstructingTiles, bool draw = false)
        {
            // Emergency check: it once occurred that an area of size 1 got spawned a solid rock..
            if (Size == 1) return true;
            
            // Check collision/overlap with connections:
            if (obstructingTiles != null) {
                foreach (var connection in Connections) {
                    if (connection.Nodes.Intersect(obstructingTiles).Count() > 0)
                        return true;
                }
            }

            // Check inner accesibility of all connections:
            if (Connections.Count() > 1)
            {
                // Iterate through all connections
                for (int i = 0; i < Connections.Count() - 1; i++)
                {
                    var current = Connections[i].Nodes;
                    var next = Connections[i + 1].Nodes;

                    if (!IsReachable(current, next, obstructingTiles, draw))
                        return true;
                }
            }

            // Check accesibility of other important objects
            foreach (var tiles in ImportantTileSets)
                if (!IsAccessible(tiles, obstructingTiles, draw))
                    return true;

            return false;
        }

        /// <summary>
        /// This function checks whether specified instance is reachable from the nodes of the first connection.
        /// (By definition this means accessibility from ALL connections.)
        /// </summary>
        public virtual bool IsAccessible(List<Vector> targetTiles, List<Vector> obstructingTiles = null, bool draw = false)
        {
            // First try any of the connections, if none, try any of the important objects around.
            var startingTiles = Connections.FirstOrDefault()?.Nodes ?? ImportantTileSets.FirstOrDefault();
            if (startingTiles == null || startingTiles.Count <= 0)
                return true;

            //var startingTiles = Connections[0].Nodes;
            return IsReachable(startingTiles, targetTiles, obstructingTiles, draw);
        }

        /// <summary>
        /// Tries to spawn the instance at a certain location: this being the instance's current coordinates!
        /// </summary>
        /// <param name="inst">The instance to spawn.</param>
        /// <param name="mustBeReachable">Whether this new instance must be reachable at all times by the player.</param>
        /// <returns>Whether the spawn succeeded.</returns>
        public bool TrySpawnInstance(Instance inst, bool mustBeReachable = false)
        {
            if (CanSpawnInstance(inst, mustBeReachable))
            {
                // Create instance
                Level.CreateInstance(inst);

                // Remove occupying tiles from free nodes
                foreach (var t in inst.GetTiles())
                    FreeNodes.Remove(t);

                // Add instance to important list if must be reachable
                if (mustBeReachable)
                    ImportantTileSets.Add(inst.GetTiles().ToList());

                return true;
            }

            return false;
        }

        public void RemoveInstance(Instance inst)
        {
            Level.RemoveInstance(inst);
            FreeNodes.AddRange(inst.GetTiles());
            ImportantTileSets.Remove(inst.GetTiles().ToList());
        }

        /// <summary>
        /// Checks whether the instance can be spawned at the instance's current coordinates.
        /// </summary>
        /// <param name="inst">The instance to spawn.</param>
        /// <param name="mustBeReachable">Whether this new instance must be reachable at all times by the player.</param>
        /// <returns>Whether the instance can be spawned here.</returns>
        public bool CanSpawnInstance(Instance inst, bool mustBeReachable = false)
        {
            // Check to see whether instance can stand there:
            var nodes = inst.GetTiles().ToList();
            foreach (var node in nodes)
            {
                if (!AccessibilityChecker.IsWalkable(Level, node) || !FreeNodes.Contains(node))
                    return false;
            }

            // Check to see whether instance obstructs crucial tiles
            if (CheckInstanceObstructs(inst))
            {
                return false;
            }

            // Check to see whether instance is reachable if is required
            if (mustBeReachable && !IsAccessible(nodes))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Pass-on function. Tries to find suitable location and spawns instance if found.
        /// </summary>
        /// <returns>Whether succesful.</returns>
        public bool FindInstanceSpawn(Instance inst, bool mustBeReachable = false)
        {
            return FindInstanceSpawn(inst, null, 0, true, mustBeReachable);
        }

        /// <summary>
        /// Finds a valid spawn location for provided instance.
        /// Adds it to the active instances list and will remove the nodes from the free nodes of this area.
        /// </summary>
        /// <param name="inst">The instance to spawn.</param>
        /// <param name="mustBeReachable">Whether this new instance must be reachable at all times by the player.</param>
        /// <returns>Whether the spawn succeeded</returns>
        public bool FindInstanceSpawn(Instance inst, Vector? position, int preferredRange, bool spawnAnyways, bool mustBeReachable = false)
        {
            if (inst == null)
                return false;

            // Try to find a suitable position for the instance
            if (position != null)
            {
                var inRange = FreeNodes.Where(n => (n - position.Value).ChebyshevLength() < preferredRange).ToList();
                foreach (var node in inRange)
                {
                    inst.Position = node;
                    if (TrySpawnInstance(inst, mustBeReachable))
                        return true;
                }
            }

            // If failed to spawn in range: allow to be spawned somewhere else, or no preferred position:
            if (position == null || spawnAnyways)
            {
                var nodes = position != null ? FreeNodes.SortAround(position.Value) : FreeNodes;
                foreach (var node in nodes)
                {
                    inst.Position = node;
                    if (TrySpawnInstance(inst, mustBeReachable))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks whether the block id can be placed at the specified location. I.e. is everything important still accessible?
        /// </summary>
        /// <param name="node">The position.</param>
        /// <param name="blockID">The block id.</param>
        /// <returns>Whether it can be placed at that position.</returns>
        public bool CanCreateTile(Vector node, int blockID)
        {
            return !CheckTileObstructs(node, blockID);
        }

        /// <summary>
        /// Attempts to place the block at the specified position.
        /// Fails if this block obstructs crucial paths.
        /// </summary>
        /// <param name="node">The location.</param>
        /// <param name="blockID">The block id to place</param>
        /// <param name="isFloor">Whether to place the block as a floor or object.</param>
        /// <param name="checkObstruction">Whether it should be checked if it obstructs crucial inner area paths.</param>
        /// <param name="removeNonSolidObjectNodes">Whether the corresponding FreeNode must be removed if it is a non-solid object tile.</param>
        /// <returns>Whether it could be successfully created.</returns>
        public bool TryCreateTile(Vector node, int blockID, bool isFloor = true, bool checkObstruction = false, bool removeNonSolidObjectNodes = true, bool requiresFreeNode = false)
        {
            if (requiresFreeNode && !FreeNodes.Contains(node))
                return false;
            
            if (checkObstruction && !CanCreateTile(node, blockID))
                return false;

            // Remove free node if player can't walk over tile or tile is an object
            if (!isFloor && (removeNonSolidObjectNodes || !BlockID.GetData(blockID).IsWalkable) || isFloor && !BlockID.GetData(blockID).IsWalkable)
                FreeNodes.Remove(node);

            // Create tile
            if (isFloor)
                Level.SetGround(node, new Block(blockID));
            else
                Level.SetObject(node, new Block(blockID));

            return true;
        }

        public bool FindTileSpawn(int blockID, bool isFloor, Vector? position, int preferredRange, bool spawnAnyways, bool checkObstruction)
        {
            return FindTileSpawn(blockID, isFloor, position, preferredRange, spawnAnyways, checkObstruction, out _);
        }
        public bool FindTileSpawn(int blockID, bool isFloor, Vector? position, int preferredRange, bool spawnAnyways, bool checkObstruction, out Vector pos)
        {
            // Try to find a suitable position for the tile
            if (position != null)
            {
                var inRange = FreeNodes.Where(n => (n - position.Value).ChebyshevLength() < preferredRange).ToList();
                foreach (var node in inRange)
                {
                    if (TryCreateTile(node, blockID, isFloor, checkObstruction))
                    {
                        pos = node;
                        return true;
                    }
                }
            }

            // If failed to spawn in range: allow to be spawned somewhere else, or no preferred position:
            if (position == null || spawnAnyways)
            {
                var nodes = position != null ? FreeNodes.SortAround(position.Value) : FreeNodes;
                foreach (var node in nodes)
                {
                    if (TryCreateTile(node, blockID, isFloor, checkObstruction))
                    {
                        pos = node;
                        return true;
                    }  
                }
            }

            pos = Vector.Zero;
            return false;
        }



        // === Population: Area Style Assignment === \\

        public virtual void Populate(Random random, Populator populator)
        {
            if (this is Pathway) return;

            // Generate the area according to the set area generator
            Generator?.PopulateArea(this, random);

            // Populate the area with a random secondary generation style:
            var sec = AreaGeneration.AreaData.PickSecondaryAreaType(this, Level.Depth, Level.Temp, random);
            if (sec != null)
            {
                var gen = (AreaGeneration.AreaGenerator)Activator.CreateInstance(sec);
                gen.PopulateArea(this, random);
            }

            // Spawn barrels:
            //SpawnBarrels(random);

            // Spawn graves
            if (random.NextDouble() < 0.02)
            {
                var gravePos = FreeNodes.PickRandom(random);
                FreeNodes.SortAround(gravePos);
                int amtOfGraves = 4 + random.Next(4);
                for (int i = 0; i < amtOfGraves; i++)
                {
                    FindInstanceSpawn(new Grave(0, 0), gravePos, 3, true, false);
                }
            }


            if (IsStart || Generator != null && !Generator.AddSpawners)
                return;

            // Assign a handful of enemy spawners:
            var nodes = GetCenterFreeNodes();
            int maxAmt = Math.Max(1, Size / 25);
            for (int i = 0; i < maxAmt; i++)
            {
                // Find a position such that this eventlocation is far enough away from other events.
                var pos = nodes.Where(n => EventLocations.All(e => (e-n).ChebyshevLength() >= 5)).ToList().PickRandom(random);
                if (pos != Vector.Zero)
                {
                    populator.AddSpawner(AssignSpawner(pos, random));
                    EventLocations.Add(pos);
                }
            }
        }

        public virtual void SpawnBarrels(Random random)
        {
            while (random.NextDouble() <= 0.20)
            {
                if (Level.BenefitPoints < 2)
                    break;

                var item = LootGenerator.GetLoot("loot_barrel", Level.Depth, random);
                if (item != null && item.Length > 0)
                {
                    var barrel = new Box(0, 0, item);
                    var pos = FreeNodes.FirstOrDefault(n => EdgeNodes.Exists(e => (e-n).OrthogonalLength() == 1));
                    if (FindInstanceSpawn(barrel, pos, 1, false, true))
                    {
                        Level.LevelManager.RemoveBenefit(2);
                    }
                }
            }
        }

        /// <summary>
        /// Assigns a random spawner to be returned.
        /// </summary>
        private Spawner AssignSpawner(Vector pos, Random random)
        {
            //return new Spawners.Living(this, pos, random);

            var roll = random.NextDouble();

            if (roll < 0.95)//0.60
                return new Spawners.Living(this, pos, random);
            //else if (roll < 0.85)
            //    return new Spawners.Lair(this, pos, random);
            else
                return new Spawners.Haunted(this, pos, random);
        }
    }
}
