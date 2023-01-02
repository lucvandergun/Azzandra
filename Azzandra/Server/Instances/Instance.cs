using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    /// <summary>
    /// The most general type of the instances. All instances have / are able to:
    ///  - Positioning & movement methods
    ///  - Movement animations
    ///  - Initiative handling
    ///  - A unique ID (identifier) when spawned
    ///  - Saving & loading methods
    ///  - A parent instance reference and or child instance references
    ///  - Collision / distance / range checker methods
    ///  - Rendering properties and methods
    /// </summary>
    public abstract class Instance
    {
        public virtual bool RenderFire => false;
        public virtual bool RenderFrozen => false;
        public virtual bool RenderLightness => true;
        public virtual float Angle => 0f;
        public AnimationManager AnimationManager;

        public virtual string AssetName => GetType().Name.ToUnderscore();
        public virtual Color AssetLightness => Color.White;
        public virtual Texture2D GetSprite() => Assets.GetSprite(AssetName);

        public Level Level { get; set; }
        //public bool IsDestroyed { get; private set; } = false;
        public int MoveTimer { get; set; } = 0; // Counts down.
        public virtual int GetMoveDelay() => 1;

        /// <summary>
        /// The parent instance of this.
        /// </summary>
        public InstRef Parent { get; set; }
        /// <summary>
        /// List of instance children, used for keeping track of long-term projectiles, spell effects or other combatants spawned by this combatant, etc.
        /// </summary>
        public List<InstRef> Children { get; protected set; } = new List<InstRef>();
        public InstRef CreateRef() => new InstRef(this);


        // === Initiative Handling === \\

        /// <summary>
        /// The time it takes until an action may be performed.
        /// </summary>
        public virtual int Initiative { get; protected set; } = 8;   // 8 is the basic unit, the player will have this speed under normal circumstances.
        /// <summary>
        /// The current potential for taking actions. One action or 'Turn' can be taken per initiative. If not enough potential present, it will have to be accumulated first.
        /// </summary>
        public int ActionPotential { get; set; } = 0;
        /// <summary>
        /// Checks whether ActionPotential is greater than the Initiative.
        /// </summary>
        public bool CanPerformTurn() => ActionPotential >= Initiative;

        /// <summary>
        /// The time in action potential built up since the last turn: should be equal to 'ActionPotential', but this counter is 
        /// </summary>
        public int TimeSinceLastTurn { get; set; } = 0;
        
        public int MomentOfLastTurn = 0;

        /// <summary>
        /// The amount of ticks until this creature gets destroyed. Creatures with a positive death timer won't be saved.
        /// (Will be set to the attacker's initiative when killed.)
        /// </summary>
        public int DeathTimer { get; set; } = -1;


        // === Attributes === \\
        public int ID { get; set; } = -1;
        public int X { get; protected set; }
        public int Y { get; protected set; }

        public Vector Position
        {
            get => new Vector(X, Y);

            set
            {
                X = value.X; Y = value.Y;
                //PreviousPosition = Position;
            }
        }
        //public Vector PreviousPosition { get; protected set; }
        //public bool HasMovedSinceLastTurn() => Position != PreviousPosition;

        public virtual int GetW() { return 1; }
        public virtual int GetH() { return 1; }
        public Vector Size => new Vector(GetW(), GetH());

        public virtual IEnumerable<Vector> GetTiles()
        {
            var list = new List<Vector>();
            if (GetW() == 1 && GetH() == 1)
                list.Add(Position);
            else
            {
                for (int x, y = Y; y < Y + GetH(); y++)
                    for (x = X; x < X + GetW(); x++)
                        list.Add(new Vector(x, y));
            }
            return list;
        }
        
        public virtual bool IsSolid() { return true; }
        public virtual bool BlocksLight() { return false; }
        public virtual bool IsInteractable() { return false; }
        public virtual bool IsInInteractionRange(Entity entity) { return IsTouchingOrColliding(entity); }
        public virtual bool IsAttackable() => false;
        public virtual bool CanBeTargetedByPlayer() => IsAttackable() || IsInteractable();
        public virtual bool CanBeDestroyed() => true;

        public Instance(int x, int y)
        {
            X = x;
            Y = y;
            ActionPotential = 0;
        }


        // === Saving & Loading === \\
        // Saving and loading is top-down: from the child class all the way to this super class.
        // - Overriding Load(...): call base.Load(...) at the end.
        // - Overriding ToBytes(...): return newArray.Concat(base.ToBytes(...)).

        public virtual void Load(byte[] bytes, ref int pos)
        {
            // Instance id
            ID = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Position
            X = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Y = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // ActionPotential
            ActionPotential = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // MoveTimer
            MoveTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Parent
            Parent = InstRef.Load(BitConverter.ToInt32(bytes, pos));
            pos += 4;

            // Children
            int childAmt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Children = new List<InstRef>(childAmt);
            for (int i = 0; i < childAmt; i++)
            {
                Children.Add(InstRef.Load(BitConverter.ToInt32(bytes, pos)));
                pos += 4;
            }

            SetupAnimationManager();
            //base.Load(bytes, ref pos);
        }

        public virtual byte[] ToBytes()
        {
            var bytes = new byte[24];
            int pos = 0;

            // Instance id
            bytes.Insert(pos, BitConverter.GetBytes(ID));
            pos += 4;

            // Position
            bytes.Insert(pos, BitConverter.GetBytes(X));
            pos += 4;
            bytes.Insert(pos, BitConverter.GetBytes(Y));
            pos += 4;

            // ActionPotential
            bytes.Insert(pos, BitConverter.GetBytes(ActionPotential));
            pos += 4;

            // MoveTimer
            bytes.Insert(pos, BitConverter.GetBytes(MoveTimer));
            pos += 4;
            if (this is Ooze) Debug.WriteLine("mt saved:" + MoveTimer);

            // Parent
            bytes.Insert(pos, BitConverter.GetBytes(InstRef.GetSaveID(Parent)));
            pos += 4;

            // Children
            int childAmt = Children.Count();
            var childBytes = BitConverter.GetBytes(childAmt);
            foreach (var c in Children)
            {
                childBytes = childBytes.Concat(BitConverter.GetBytes(InstRef.GetSaveID(c))).ToArray();
            }
            bytes = bytes.Concat(childBytes).ToArray();

            return bytes; //.Concat(base.ToBytes());
        }

        IEnumerable<Vector> SightSquares;
        public void CheckSightLine(Vector start, Vector end)
        {
            var ray = start.CastRay(end, true, true);
            SightSquares = ray;
        }


        // Methods:

        /// <summary>
        /// Is called right after the instance is first added to the level. Could be overridden to set the enemy's hp to it's loaded full hp, etc.
        /// Is not called when instances are loaded from the save file!
        /// </summary>
        public virtual void Init()
        {
            
        }

        /// <summary>
        /// Is the very last thing called when instances are spawned or loaded.
        /// Is used e.g. for setting up the asset.!
        /// </summary>
        public virtual void SetupAnimationManager()
        {
            AnimationManager = new AnimationManager(AssetName)
            {
                RenderLightness = () => RenderLightness,
                RenderFire = () => RenderFire,
                RenderFrozen = () => RenderFrozen,
                Angle = () => Angle
            };
        }

        public void DestroyNextTurn()
        {
            DeathTimer = Initiative;
        }

        public void Destroy()
        {
            //IsDestroyed = true;
            ApplyDeathEffects();
            Level?.RemoveInstance(this);
        }

        protected virtual void ApplyDeathEffects()
        {
            Animations.Clear();
        }

        /// <summary>
        /// Update timers and check for destruction here.
        /// </summary>
        public virtual void TurnStart()
        {
            //Animations.Clear();
            SightSquares = null;

            // Rebound move timer
            if (MoveTimer > 0) MoveTimer--;
            //PreviousPosition = Position;

            // Perform collision-BY actions with other instances
            var colliding = Level.ActiveInstances.Where(i => IsCollisionWith(i)).ToList();
            foreach (var inst in colliding)
            {
                if (inst != this)
                    OnCollisionByInstance(inst);
                //inst.OnCollisionByInstance(this);
            }
        }

        /// <summary>
        /// Perform any AI decisions here (e.g. set Combatant's .Action property), as well as executing them.
        /// </summary>
        public virtual void Turn()
        {
            // Perform collision-WITH actions with other instances
            var colliding = Level.ActiveInstances.Where(i => IsCollisionWith(i)).ToList();
            foreach (var inst in colliding)
            {
                if (inst != this)
                    OnCollisionWithInstance(inst);
                //inst.OnCollisionByInstance(this);
            }
        }

        /// <summary>
        /// State any handling to be done after the entire tick has passed. Method is executed just before the next tick.
        /// </summary>s
        public virtual void TurnEnd()
        {
            //Animations.Clear();
        }


        /// <summary>
        /// Returns true if instance can stand/exist on a position. (Used for teleportation/spawning, etc.)
        /// </summary>
        public virtual bool CanExist(int nx, int ny)
        {
            if (Level == null)
                return true;

            // Out of world bounds
            if (nx < 0 || ny < 0 || nx + GetW() > Level.MapWidth || ny + GetH() > Level.MapHeight)
                return false;


            // TileMap collision
            var ownTiles = GetTiles().Select(t => t + new Vector(nx - X, ny - Y));
            foreach (var node in ownTiles)
            {
                var tile = Level.TileMap[node.X, node.Y];
                if (!CanWalkOverBlock(tile.Ground) || !CanWalkOverBlock(tile.Object))
                    return false;
            }

            // Instance collision
            foreach (var inst in Level.ActiveInstances)
            {
                if (inst != this && IsInstanceSolidToThis(inst) && ownTiles.Intersect(inst.GetTiles()).Count() > 0)
                    return false;
            }

            //for (int x, y = ny; y < ny + GetH(); y++)
            //{
            //    for (x = nx; x < nx + GetW(); x++)
            //    {
            //        var tile = Level.TileMap[x, y];
            //        if (!CanWalkOverBlock(tile.Ground) || !CanWalkOverBlock(tile.Object))
            //            return false;
            //    }
            //}
            //foreach (var inst in Level.ActiveInstances)
            //{
            //    if (IsInstanceSolidToThis(inst))
            //    {
            //        if (inst.IsInRegion(nx, ny, nx + GetW(), ny + GetH()))
            //            return false;
            //    }
            //}

            return true;
        }

        /// <summary>
        /// Checks whether this instance should be able to exist on a block. Used by Instance.CanExist().
        /// </summary>
        public virtual bool CanWalkOverBlock(Block block) => block.Data.IsWalkable;

        /// <summary>
        /// Checks whether this instance should be able to exist on another instance. Used by Instance.CanExist().
        /// </summary>
        public virtual bool IsInstanceSolidToThis(Instance inst) => IsSolid() && inst.IsSolid();



        /// <summary>
        /// This method performs on-collision effects with another instance, as seen FROM this instance. 
        /// E.g. a gelatinous cube moves onto a spot and collides onto the other. 
        /// Should therefore be called AFTER ticking.
        /// </summary>
        /// <param name="inst"></param>
        public virtual void OnCollisionWithInstance(Instance inst)
        {
            // Pass on to collision-BY method, this allows this specific entity to override any effects performed by the colliding instance.
            inst.OnCollisionByInstance(this);
        }

        /// <summary>
        /// This method is called and performs on-collision effects as seen as TOWARDS this instance.
        /// </summary>
        /// <param name="collider"></param>
        public virtual void OnCollisionByInstance(Instance collider)
        {

        }




        public virtual void Interact(Entity entity) { }

        /// <summary>
        /// Creates grounditem instance of specified item under instance.
        /// </summary>
        public void DropItem(Item item)
        {
            // Separate amount if not stackable
            if (!item.Stack && item.Quantity > 1)
                for (int i = 0; i < 10 && i < item.Quantity; i++)
                    Level.CreateInstance(new GroundItem(X, Y, (Item)Activator.CreateInstance(item.GetType())));
            else
                Level.CreateInstance(new GroundItem(X, Y, item));
        }




        // === Position Checkers === \\

        /// <summary>
        /// Returns true if both instances are overlapping.
        /// </summary>
        public bool IsCollisionWith(Instance other)
        {
            return GetTiles().Intersect(other.GetTiles()).Count() >= 1;
        }

        /// <summary>
        /// Returns true if instance is in specified region for any of its tiles. (x1, x2 are inclusive, x2, y2 are not.)
        /// </summary>
        public virtual bool IsInRegion(int x1, int y1, int x2, int y2)
        {
            return GetTiles().Any(t =>
                t.X >= x1 &&
                t.X < x2 &&
                t.Y >= y1 &&
                t.Y < y2
                );
        }
        public virtual bool IsInRegion(Region region)
        {
            return GetTiles().Any(t =>
                t.X >= region.Position.X &&
                t.X < region.BottomRight.X &&
                t.Y >= region.Position.Y &&
                t.Y < region.BottomRight.Y
                );
        }

        /// <summary>
        /// Returns true if specified instance is in range of calling instance for any of its tiles.
        /// Range includes calling instances' size.
        /// </summary>
        public bool IsInRange(Instance other, int range)
        {
            // Square range: in range = if for any of the caller's tiles there exists a tile in target that is in range.
            return GetTiles().Any(t => other.GetTiles().Any(t2 => 
                Math.Abs((t2 - t).X) <= range && Math.Abs((t2 - t).Y) <= range));
        }

        /// <summary>
        /// Returns true if specified instance is (partially) in range of region.
        /// </summary>
        public bool IsInRangeFromPoint(Vector? centre, int range)
        {
            if (centre == null)
                return true;

            var pos = centre.Value;
            return IsInRegion(pos.X - range, pos.Y - range, pos.X + range + 1, pos.Y + range + 1);
        }

        /// <summary>
        /// Returns true if specified instance is next to but not overlapping with calling instance.
        /// </summary>
        public bool IsTouching(Instance other)
        {
            // If for any tiles of caller, there is a tile in other that is exactly 1 tile away. But not colliding!
            if (IsCollisionWith(other))
                return false;

            return GetTiles().Any(t => other.GetTiles().Any(t2 =>
                (t2 - t).OrthogonalLength() == 1));
        }

        public bool IsTouchingOrColliding(Instance other) => IsTouching(other) || IsCollisionWith(other);

        /// <summary>
        /// Calculates the distance between both instances. When touching distance = 1, overlapping distance = 0.
        /// </summary>
        public Vector DistanceTo(Instance other)
        {
            if (other == null)
                return Vector.Zero;
            //var query = firstList.SelectMany(x => secondList, (x, y) => new { x, y });

            // For all tiles in 1 to all tiles in 2, select min distance:
            Vector? minDist = null;
            foreach (var t1 in GetTiles())
            {
                foreach (var t2 in other.GetTiles())
                {
                    if (!minDist.HasValue || (t2 - t1).OrthogonalLength() < minDist.Value.OrthogonalLength())
                        minDist = (t2 - t1);
                }
            }

            return minDist.Value;
        }

        /// <summary>
        /// Calculates the distance between an instance and a certain point. When touching distance = 1, overlapping distance = 0.
        /// </summary>
        public Vector DistanceTo(Vector target)
        {
            // For all tiles in 1 to all tiles in 2, select min distance:
            Vector? minDist = null;
            foreach (var t1 in GetTiles())
            {
                if (!minDist.HasValue || (target - t1).OrthogonalLength() < minDist.Value.OrthogonalLength())
                    minDist = (target - t1);
            }

            return minDist.Value;
        }

        /// <summary>
        /// Calculates the Chebyshev Distance between the instances.
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int TileDistanceTo(Instance other)
        {
            if (other == null) return -1;
            return DistanceTo(other).ChebyshevLength();
        }

        /// <summary>
        /// Returns true if entity is colliding with instance OR entity is touching and instance is solid.
        /// </summary>
        public virtual bool IsEntityInInteractionRange(Entity entity)
        {
            if (IsSolid())
                return entity.IsTouching(this);
            else
                return entity.IsCollisionWith(this);
        }


        // E.g. 'FireGiant' class returns 'fire giant'. Override to provide custom (capitalised) name.
        public virtual string Name => string.Concat(GetType().Name.Select((x, i) => i > 0 && char.IsUpper(x)? " " + x.ToString() : x.ToString())).ToLower();
        public virtual string SecretName => Name;
        public override string ToString() => Name;

        public virtual string ToStringAdress()
        {
            var name = ToString();
            return char.IsUpper(name[0]) ? name : "the " + name;
        }

        public virtual string ToStringArticle()
        {
            var name = ToString();
            return char.IsUpper(name[0]) ? name : Util.AddArticle(name);
        }

        public virtual string ToSecretStringArticle()
        {
            var name = SecretName;
            return char.IsUpper(name[0]) ? name : Util.AddArticle(name);
        }


        public virtual string GetInfo()
        {
            return "Unknown object.";
        }



        public virtual Affect GetAffected(Entity attacker, Affect affect)
        {
            // Roll success
            bool success = affect.RollSuccess(attacker, this);

            // Apply affect and its properties
            if (success) affect.Apply(attacker, this);

            affect.AddMainMessage(attacker, this);

            if (success) affect.ApplyProperties(attacker, this);

            affect.ShowMessages();

            return affect;
        }












        // Properties:
        public virtual int GetMovementSpeed() => 1;
        public List<IAnimation> Animations { get; protected set; } = new List<IAnimation>();

        public enum MoveType { Walk, Fly };
        public virtual MoveType GetMovementType() => MoveType.Walk;
        public bool IsGrounded => GetMovementType() == MoveType.Walk;
        public virtual bool SlidesOnIce() => GetMovementType() == MoveType.Walk;
        public virtual bool CanMoveDiagonal() => true;


        // === Movement === \\
        public virtual List<Vector> Move(Vector distance, bool orthoDiagonal = true, bool hasSlided = false, bool mergeMovements = false)
        {
            // Skip function if no movement
            if (distance == Vector.Zero)
                return new List<Vector>();

            return Move(new Vector[] { distance }, orthoDiagonal, hasSlided, mergeMovements);
        }

        public virtual List<Vector> Move(IEnumerable<Vector> distances, bool orthoDiagonal = true, bool hasSlided = false, bool mergeMovements = false)
        {
            //// Skip function if no movement
            //if (distance == Vector.Zero)
            //    return new List<Vector>();

            //PreviousPosition = Position;
            var oldPos = Position;

            // Divide movement into segments of distance 1
            var distsToGo = distances.ToList().CreateCopy();
            var amtOfSteps = distances.Select(v => v.ChebyshevLength()).Aggregate((a, b) => a + b);

            List<Vector> steps = new List<Vector>();

            // Move step by step:
            while (amtOfSteps > 0)
            {
                amtOfSteps--;
                var step = distsToGo.First().Sign();

                // If diagonal movement is not possible: edit the step to either dimensional component (solely X or Y):
                if (step.X != 0 && step.Y != 0 && (orthoDiagonal && !CanMoveUnobstructed(step.X, step.Y) || !CanMoveDiagonal()))
                {
                    var remainder = distsToGo.Aggregate((a, b) => a + b).Absolute();
                    if (remainder.X >= remainder.Y)
                    {
                        if (CanMoveUnobstructed(step.X, 0))
                            step.Y = 0;
                        else if (CanMoveUnobstructed(0, step.Y))
                            step.X = 0;
                    }
                    else
                    {
                        if (CanMoveUnobstructed(0, step.Y))
                            step.X = 0;
                        else if (CanMoveUnobstructed(step.X, 0))
                            step.Y = 0;
                    }
                }

                if (CanMoveUnobstructed(step.X, step.Y))
                {
                    Position += step;

                    // On instance walking is sent towards both the entity and the tile
                    if (IsGrounded)
                    {
                        foreach (var node in GetTiles())
                        {
                            var tile = Level.GetTile(node);
                            tile.OnInstanceWalking(Level, node, this);
                            StepOnTile(tile, node);
                        }
                    }

                    // Add value to movement tracker
                    if (step != Vector.Zero)
                        steps.Add(step);

                    // Remove step from current distance-to-go
                    distsToGo[0] -= step;
                    if (distsToGo[0] == Vector.Zero)
                        distsToGo.RemoveAt(0);
                }
            }

            // If the entity ends up on all-ice: add forced action for next turn!
            if (!hasSlided && SlidesOnIce() && GetTiles().All(t => Level.GetTile(t).Ground.ID == BlockID.Ice))
            {
                var distTraversed = Position - oldPos;
                var slideStep = distTraversed.Sign();
                if (CanMoveUnobstructed(slideStep.X, slideStep.Y))
                {
                    if (this is Entity cb && !(this is CaveWormTail))
                    {
                        var iceSlide = new ActionMove(cb, distTraversed)
                        {
                            IsForced = true,
                            HasSlided = true
                        };
                        cb.NextAction = iceSlide;
                    }
                }
            }

            var tt = this;

            // Merge all movements to one, if requested:
            if (mergeMovements && steps.Count > 0)
                steps = new List<Vector>() { steps.Aggregate((cur, val) => val + cur) };

            Animations.Add(new MovementAnimation(this, steps, Initiative));
            return steps;
        }

        protected Vector GetStepTowards(Instance inst)
        {
            // Get distance and boundarize it by movement speed for each axis.
            var dist = DistanceTo(inst);
            return Vector.Smallest(dist, dist.Sign() * GetMovementSpeed());
        }

        /// <summary>
        /// Returns true if entity can move with specified amount from its current position.
        /// </summary>
        public bool CanMoveUnobstructed(int dx, int dy)
        {
            return CanMoveUnobstructed(X, Y, dx, dy, true);
        }
        /// <summary>
        /// Returns true if entity can move with specified amount from specified position.
        /// </summary>
        public virtual bool CanMoveUnobstructed(int x, int y, int dx, int dy, bool incorporateEntities = true)
        {
            // Return true if no movement
            if (Level == null || dx == 0 && dy == 0)
                return true;

            // Sign movement amount
            dx = Math.Sign(dx);
            dy = Math.Sign(dy);

            // Calculate movement region
            int x1 = Math.Min(x, x + dx);
            int y1 = Math.Min(y, y + dy);
            int x2 = x1 + GetW() + Math.Abs(dx);
            int y2 = y1 + GetH() + Math.Abs(dy);

            // Out of world bounds
            if (x1 < 0 || y1 < 0 || x2 > Level.MapWidth || y2 > Level.MapHeight)
                return false;

            // TileMap collisions:
            Tile tile;
            for (int i, j = y1; j < y2; j++)
            {
                for (i = x1; i < x2; i++)
                {
                    // Don't count tiles under entity
                    if (i >= x && j >= y && i < x + GetW() && j < y + GetH())
                        continue;

                    bool isCorner = false;
                    // Allow for solid corner tiles that aren't under origional pos and under new pos
                    if (!(i >= x + dx && j >= y + dy && i < x + dx + GetW() && j < y + dy + GetH()))
                    {
                        isCorner = true;
                    }

                    tile = Level.TileMap[i, j];

                    // Determine whether can move on tile depending on movement mode
                    bool isWalk = GetMovementType() == MoveType.Walk; // && isGrounded - Could be additional parameter to e.g. 'blow away' walking entity.
                    var canMoveOnFloor = isWalk ? CanWalkOverBlock(tile.Ground) : CanFlyOverBlock(tile.Ground);
                    var canMoveOnObject = isWalk ? CanWalkOverBlock(tile.Object) : CanFlyOverBlock(tile.Object);

                    if (canMoveOnFloor && canMoveOnObject ||
                        !canMoveOnFloor && canMoveOnObject && isCorner && CanBlockBeCornered(tile.Ground) ||
                        canMoveOnFloor && !canMoveOnObject && isCorner && CanBlockBeCornered(tile.Object) ||
                        !canMoveOnFloor && !canMoveOnObject && isCorner && CanBlockBeCornered(tile.Ground) && CanBlockBeCornered(tile.Object))
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //instance collision
            foreach (var inst in Level.ActiveInstances)
            {
                if (IsInstanceSolidToThis(inst) && (!(inst is Entity) || incorporateEntities))
                {
                    /*
                    //total movement area - origional area
                    if (inst.IsInRegion(x1, y1, x2, y2) && !inst.IsInRegion(x, y, x + GetW(), y + GetH()))
                        return false;
                    */

                    //if in new area and not in old area
                    if (inst.IsInRegion(x + dx, y + dy, x + dx + GetW(), y + dy + GetH()) && !inst.IsInRegion(x, y, x + GetW(), y + GetH()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public virtual bool CanStandOnTile(Tile tile, MoveType moveType)
        {
            if (moveType == MoveType.Walk)
            {
                return CanWalkOverBlock(tile.Ground) && CanWalkOverBlock(tile.Object);
            }
            else
            {
                return CanFlyOverBlock(tile.Ground) && CanFlyOverBlock(tile.Object);
            }
        }

        public virtual bool CanTileBeCornered(Tile tile)
        {
            return CanBlockBeCornered(tile.Ground) && CanBlockBeCornered(tile.Object);
        }


        /// <summary>
        /// Checks whether tile is walkable for this entity.
        /// </summary>
        public virtual bool CanFlyOverBlock(Block block) => block.Data.IsFlyable;

        public virtual bool CanBlockBeCornered(Block block) => block.Data.IsCornerable;

        /// <summary>
        /// Perform on-step actions for certain tiles here. This method should only contain actions that affect the entity.
        /// </summary>
        public virtual void StepOnTile(Tile tile, Vector pos)
        {
            StepOnBlock(tile.Ground, new BlockPos(pos, true));
            StepOnBlock(tile.Object, new BlockPos(pos, false));
        }

        public virtual void StepOnBlock(Block block, BlockPos pos)
        {

        }

        /// <summary>
        /// Perform on-stand actions (is called every tick) for certain tiles here. This method should only contain actions that affect the entity.
        /// Method is called every turn.
        /// </summary>
        public virtual void StandOnTile(Tile tile, Vector pos)
        {
            StandOnBlock(tile.Ground, new BlockPos(pos, true));
            StandOnBlock(tile.Object, new BlockPos(pos, false));
        }

        public virtual void StandOnBlock(Block block, BlockPos pos)
        {

        }










        // === Rendering === \\
        public virtual Symbol GetSymbol() => new Symbol('?', Color.Orange);

        /// <summary>
        /// Calculates the absolute coordinate position according to the GRID_SIZE.
        /// The centre of the instance is returned here as well.
        /// May not be overridden, resulting in returning the static absolute positon, without incorporating movement.
        /// </summary>
        /// <returns>The absolute coordinates according to the GRID_SIZE only.</returns>
        public Vector2 GetAbsoluteStaticPos()
        {
            return (Position.ToFloat() + Size.ToFloat() / 2f) * ViewHandler.GRID_SIZE;
        }

        /// <summary>
        /// Calculates the absolute coordinate position according to the GRID_SIZE.
        /// The centre of the instance is returned.
        /// This function may be overwritten to incorporate e.g. movement using the current tick delay.
        /// </summary>
        /// <returns>The absolute coordinates according to the GRID_SIZE and the current tick delay.</returns>

        public virtual Vector2 CalculateRealPos(Server server)
        {
            var drawOffset = Vector2.Zero;
            foreach (var anim in Animations)
                //if (anim is MovementAnimation)
                    drawOffset += anim.GetDisposition();

            return GetAbsoluteStaticPos() + drawOffset;
        }

        /// <summary>
        /// Draws this instance at a position (centered) according to the given view offset.
        /// This function may be overridden to disposition it for e.g. movement.
        /// </summary>
        /// <param name="viewOffset">The current view offset to take into account (absolute coordinates of top left corner of viewing area).</param>
        public virtual void DrawView(SpriteBatch sb, Vector2 viewOffset, Server server, float lightness)
        {
            AnimationManager.Update();
            var drawPos = CalculateRealPos(server) + viewOffset;// + drawOffset;
            Draw(sb, drawPos, lightness);
        }

        /// <summary>
        /// Draws a representation of this instance at the specified position (centered).
        /// Does not draw movement disposition.
        /// </summary>
        /// <param name="pos">The position to draw at (centered).</param>
        public virtual void Draw(SpriteBatch sb, Vector2 pos, float lightness = 1f)
        {
            //var symbol = GetSymbol();
            //if (symbol == null)
            //    return;
            //var color = symbol.Color;

            //// Get sprite:
            //var sprite = GetSprite();
            //if (sprite == null) return;

            //var color = AssetLightness;
            //if (RenderFire)
            //    color = ViewHandler.FireColor;
            //else if (RenderFrozen)
            //    color = Color.LightBlue;

            //if (RenderLightness)
            //    color = color;

            //Display.DrawSprite(pos, sprite, color, 1f, 0f);

            var color = AssetLightness;
            AnimationManager.Draw(sb, pos, 1f, color, lightness);
        }
    }
}
