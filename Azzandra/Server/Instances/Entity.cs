using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Azzandra
{
    /// <summary>
    /// Entities form the next big type of instance. They resemble agents and are capable of the following:
    ///  - Health, attackability, as well as status effects
    ///  - Vision handling
    /// </summary>
    public abstract class Entity : Instance
    {
        // === General Properties === \\
        public override bool DisplayFire => HasStatusEffect(StatusEffectID.Burning);
        public override bool DisplayFrozen => StatusEffects.Exists(effect => effect.VariantName == "frozen");
        public override bool RenderLightness => false;

        public virtual int GetFullHp() => FullHp;
        public virtual EntityType EntityType => EntityType.None;
        public override bool IsSolid() => true;
        public override bool IsAttackable() => true;

        public virtual int GetVisionRange()
        {
            foreach (var se in StatusEffects)
                if (se.GetID() == StatusEffectID.Blind)
                    return Math.Min(Math.Max(0, 4 - se.Level), VisionRange);
            return VisionRange;
        }

        /// <summary>
        /// The amount of ticks until this creature gets destroyed. Creatures with a positive death timer won't be saved.
        /// (Will be set to the attacker's initiative when killed.)
        /// </summary>
        public int DeathTimer { get; set; } = -1;

        // === Behaviour Properties === \\


        // === Saved Attributes === \\
        protected int _hp;
        public int Hp { get => _hp; set => _hp = Math.Max(0, value); }
        public int FullHp { get; set; } = 1;

        public List<Hit> Hits = new List<Hit>();
        public List<StatusEffect> StatusEffects = new List<StatusEffect>();

        public int AttackTimer { get; set; } = 10; // Counts up
        protected virtual int VisionRange => 12;

        public InstRef Target { get; set; }


        // === Action Handling === \\
        private EntityAction _action;
        /// <summary>
        /// The current action set to be performed the next tick. Should only be set/overridden internally.
        /// </summary>
        public EntityAction Action
        {
            get => _action;
            set
            {
                // Don't allow overriding forced actions.
                if (_action == null || !_action.IsForced)
                    _action = value;
            }
        }

        /// <summary>
        /// The set action to be performed the next tick. Externally activated or 'forced' actions should be set here only.
        /// </summary>
        public EntityAction NextAction { get; set; }
        /// <summary>
        /// The action just performed (last tick). Has no influence on anything.
        /// </summary>
        public EntityAction PrevAction { get; set; }

        /// <summary>
        /// Moves the NextAction to Action. Sets NextAction to null.
        /// </summary>
        /// <returns>Whether NextAction yielded a non-null action.</returns>
        public bool PutNextAction()
        {
            PrevAction = Action;
            _action = NextAction;
            NextAction = null;
            return _action != null;
        }



        // === Combat Property Getters === \\
        protected float EffectMod(int effectID, float amtPerLvl) => TryGetStatusEffect(effectID, out var effect) ? Math.Max(0, 1f + effect.Level * amtPerLvl) : 1f;
        
        protected float WeakMod => EffectMod(StatusEffectID.Weak, -0.25f);
        protected float DamageMod => EffectMod(StatusEffectID.Strong, 0.25f);
        protected float AccuracyMod => EffectMod(StatusEffectID.Accurate, 0.25f);
        protected float EvadeMod => EffectMod(StatusEffectID.Evasive, 0.25f);
        protected float DefenseMod => EffectMod(StatusEffectID.Defensive, 0.25f);
        protected float SpellcastMod => EffectMod(StatusEffectID.Sorcerous, 0.25f);
        protected float ResistanceMod => EffectMod(StatusEffectID.Resistance, 0.25f);

        public virtual float GetPar() => DefenseMod;
        public virtual float GetBlk() => DefenseMod;
        public virtual float GetEvd() => EvadeMod;
        public virtual float GetRes() => ResistanceMod;
        public virtual float GetArm() => WeakMod;


        public Entity(int x, int y) : base(x, y)
        {
            Hp = GetFullHp();
        }


        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            // Health
            _hp = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            FullHp = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Attack timer
            AttackTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            // Target
            Target = InstRef.Load(BitConverter.ToInt32(bytes, pos));
            pos += 4;

            // Status effects
            int amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            StatusEffects = new List<StatusEffect>(amt);
            for (int i = 0; i < amt; i++)
            {
                var se = StatusEffect.Load(bytes, ref pos);
                if (se != null) StatusEffects.Add(se);
            }

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[20];
            int pos = 0;

            // Health
            bytes.Insert(pos, BitConverter.GetBytes(Hp));
            pos += 4;
            bytes.Insert(pos, BitConverter.GetBytes(FullHp));
            pos += 4;

            // Attack timer
            bytes.Insert(pos, BitConverter.GetBytes(AttackTimer));
            pos += 4;

            // Target
            bytes.Insert(pos, BitConverter.GetBytes(InstRef.GetSaveID(Target)));
            pos += 4;

            // Status effects
            int amt = StatusEffects.Count();
            bytes.Insert(pos, BitConverter.GetBytes(amt));
            pos += 4;
            foreach (var se in StatusEffects)
            {
                bytes = bytes.Concat(se.ToBytes()).ToArray();
            }

            return bytes.Concat(base.ToBytes()).ToArray();
        }



        //public override void Init()
        //{
        //    base.Init();
        //    Hp = GetFullHp();
        //}


        // Tick:
        public override void TurnStart()
        {
            base.TurnStart();
            
            // Reset turn trackers:
            SightSquares = null;
            //Hits.Clear(); // Are cleared now by themselves.

            //if (Hp <= 0)
            //{
            //    Destroy();
            //    return;
            //}

            // Rebound attack timer
            if (AttackTimer < 10) AttackTimer++;

            // Update status effects: (creating copy, because collection can be altered as they expire)
            var effects = StatusEffects.CreateCopy();
            foreach (var effect in effects)
            {
                if (effect == null)
                {
                    RemoveStatusEffect(null);
                    continue;
                }
                effect.Update(this);
            }

            // Stunned: set forced null action at tick
            if (HasStatusEffect(StatusEffectID.Stunned))
                Action = new ActionMove(this, Vector.Zero, true);

            // Check death again: set it to it's own initiative:
            if (Hp <= 0)
            {
                DeathTimer = Initiative;
                return;
            }

            // Remove target if gone:
            if (Target != null)
                if (Target.Instance?.Level != Level || !Target.Exists())
                    Target = null;

            // Update living of children
            for (int i = 0; i < Children.Count; i++)
            {
                if (!Children[i].Exists())
                {
                    Children.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void Turn()
        {
            if (!(this is Player) && Action != null)
            {
                Action.Perform();
            }

            // Sets the current action to the next(queued) action, and sets the next action to null.
            PutNextAction();
        }

        public override void TurnEnd()
        {
            base.TurnEnd();

            // Perform on-standing tile actions
            foreach (var node in GetTiles())
            {
                var tile = Level.GetTile(node);
                tile.OnInstanceStanding(Level, node, this);
                StandOnTile(tile, node);
            }

            // Perform on-collision actions with instances
            var colliding = Level.ActiveInstances.Where(i => IsCollisionWith(i)).ToList();
            foreach (var inst in colliding)
            {
                // Player instance is respawned on death... this gives complications... Maybe AddList and DeleteList in Level? (added after looping)
                // insts.onplayer tick are added at that tickstart, and the others not!
                if (inst != this)
                    OnOtherInstanceCollision(inst);
                //inst.OnInstanceCollision(this);
            }

            // Check death: set it to it's own initiative:
            if (Hp <= 0)
            {
                DeathTimer = Initiative;
                return;
            }
        }


        /// <summary>
        /// Returns true if the combatant is allowed to move (not if e.g. stunned or frozen)
        /// </summary>
        public virtual bool CanMove()
        {
            return !(HasStatusEffect(StatusEffectID.Frozen) || HasStatusEffect(StatusEffectID.Stunned));
        }

        //public override bool CanMoveUnobstructed(int x, int y, int dx, int dy, bool incorporateEntities = true)
        //{
        //    if (HasStatusEffect(StatusEffectID.Frozen) || HasStatusEffect(StatusEffectID.Stunned))
        //        return false;
        //    else
        //        return base.CanMoveUnobstructed(x, y, dx, dy, incorporateEntities);
        //}

        public override void StepOnBlock(Block block, BlockPos pos)
        {
            base.StepOnBlock(block, pos);

            switch (block.ID)
            {
                case BlockID.Cobweb:
                    if (!this.IsTypeOf(EntityType.NonPhysical) && !this.IsTypeOf(EntityType.Spider))
                    {
                        if (this is Player)
                            ((Player)this).User.Log.Add("You get stuck in the cobweb.");
                        AddStatusEffect(new StatusEffects.Frozen(1, 3, "stuck"));
                    }
                    return;
            }
        }

        public override void StandOnBlock(Block block, BlockPos pos)
        {
            base.StandOnBlock(block, pos);

            switch (block.ID)
            {
                case BlockID.Cobweb:
                    if (!this.IsTypeOf(EntityType.NonPhysical) && !this.IsTypeOf(EntityType.Spider))
                    {
                        AddStatusEffect(new StatusEffects.Frozen(1, 2, "stuck"), true);
                    }
                    return;
                case BlockID.Water:
                    if (RemoveStatusEffect(StatusEffectID.Burning) && this is Player)
                    {
                        ((Player)this).User.Log.Add("The fire surrounding you was extinguished by the water.");
                    }
                    break;
                case BlockID.Acid:
                    if (!this.IsTypeOf(EntityType.Acid))
                    {
                        GetHit(Style.Acid, 1);
                        if (this is Player p) p.User.Log.Add("<acid>The acid damages you.");
                    }
                    break;
            }
        }

        public virtual void OnOtherInstanceCollision(Instance inst)
        {
            inst.OnInstanceCollision(this);
        }


        // Combat:
        /// <summary>
        /// Heals entity with specified amount.
        /// </summary>
        /// <param name="amt">The amount to be healed.</param>
        /// <param name="canOverflow">Whether the excess over maximum health is added.</param>
        /// <returns>Amount of hitpoints restored</returns>
        public int Heal(int amt, bool canOverflow = false)
        {
            int points = Math.Min(GetFullHp() + (canOverflow ? amt : 0) - Hp, amt);
            Hp += points;
            return points;
        }

        /// <summary>
        /// Returns true if calling entity can affect specified instance.
        /// Target needs to be affectable, not in collision with, in range and in sight.
        /// </summary>
        public virtual bool CanAffect(Instance target, Affect affect)
        {
            if (target is Entity entity && entity.Hp <= 0)
                return false;
            
            if (HasStatusEffect(StatusEffectID.Stunned))
                return false;

            if (IsInstanceSolidToThis(target) && IsCollisionWith(target))
                return false;

            if (!IsInAttackRange(target, affect.Range))
                return false;

            if (!CanAimAt(target))
                return false;

            return true;
        }

        public virtual bool IsInAttackRange(Instance target, int attackRange)
        {
            // Check if entity is in range
            if (!IsInRange(target, attackRange))
                return false;

            // Check if entity is touching if range is 1
            return true;
            //return attackRange != 1 || IsTouching(target);
        }


        /// <summary>
        /// Attacks the target instance with the specified attack.
        /// Calls target.GetHit(attack) to have it take the hit.
        /// Returns the same attack but modified to have it know whether it succeeded, etc.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attack"></param>
        /// <returns></returns>
        public virtual Affect Affect(Instance target, Affect affect)
        {
            // Spawn Projectile Instance if required:
            if (affect is Attack attack)
            {
                if (attack.Style == Style.Ranged)
                    Level.CreateInstance(new ArrowProjectile(this, target));
                else if (attack.Style == Style.Magic)
                    Level.CreateInstance(new SpellProjectile(this, target, new Symbol('o', SpellProjectile.GetColor(affect.Properties))));
                //else if (attack.Style == Style.Melee)
                //    Animations.Add(new AttackAnimation(this, target));
            }

            affect = target.GetAffected(this, affect);
            AttackTimer = 0;
            return affect;
        }

        /// <summary>
        /// Actual method to remove a set damage from the hp value. Adds a hitsplat bound to this creature.
        /// </summary>
        public virtual int GetHit(Style style, int dmg)
        {
            if (HasStatusEffect(StatusEffectID.Invulnerable))
                dmg = 0;
            
            // Remove dmg value from health
            dmg = Math.Min(dmg, Hp);
            Hp -= dmg;

            // Add hitsplat:
            AddHit(new HitDmg(this, style, dmg));

            // Check death
            if (Hp <= 0)
            {
                DeathTimer = Initiative;
                //Destroy();
                //ActionPotential = -1;
                //TimeSinceLastTurn = 0;
            }

            return dmg;
        }

        /// <summary>
        /// Hits this entity by a specified attack.
        /// </summary>
        /// <param name="attacker">The origin of the attack.</param>
        /// <param name="attack">The attack used.</param>
        /// <returns></returns>
        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            /*/
             * 1. Accure attacker and its attack (with properties)
             * 2. Apply entity-specific modifiers (overridde of this)
             * 3. Apply entity type-specific modifiers
             * 4. Apply attack properties (and modifiers to attack)
             * 5. Calculate hit success & damage
             * 5. Deal damage
             * 6. Return final hit with its properties
            /*/

            // Deflect if supposed to:
            if (TryGetStatusEffect(StatusEffectID.Deflect, out var effect))
            {
                effect.Time--;
                if (effect.Time <= 0) RemoveStatusEffect(effect);
                return attacker.GetAffected(attacker, affect);
            }

            // Apply entity type-specific code:
            bool success = false;
            var hasAttackNotFailed = affect.ModifyByEntityTypes(this);
            
            if (hasAttackNotFailed)
            {
                // Roll success
                success = affect.RollSuccess(attacker, this);

                // Apply affect and its properties
                if (success) affect.Apply(attacker, this);

                affect.AddMainMessage(attacker, this);

                if (success) affect.ApplyProperties(attacker, this);
            }
            
            if (!success)
            {
                var hit = affect.CreateHitDisplay(this);
                if (hit != null) AddHit(hit);
            }

            affect.ShowMessages();

            // Show Death message:
            if (hasAttackNotFailed && Hp <= 0)
                ShowDeathMessage(GetDeathMessage(attacker));

            return affect;
        }

        public void AddHit(Hit hit)
        {
            if (hit == null) return;
            
            // Have them start not at center_y but below that?
            if (Hits.Count > 0 && Hits[0].Offset.Y <= -12)
            {
                Hits.Insert(0, hit);
            }
            else if (Hits.Count > 0 && Hits[0].Offset.Y <= -6)
            {
                hit.Offset.Y = Hits[0].Offset.Y + 12;
                Hits.Insert(0, hit);
            }
            else if (Hits.Count == 0)
            {
                //hit.Offset.Y += 12;
                Hits.Add(hit);
            }
            else
            {
                var maxOffset = Hits.Min(h => h.Offset.Y);
                hit.Offset.Y = maxOffset - 12;
                Hits.Add(hit);
            }
        }


        public virtual string GetDeathMessage(Entity attacker)
        {
            return attacker is Player
                ? "<lime>You have killed " + (attacker == this ? "yourself" : ToStringAdress()) + "."
                : "<lime>" + attacker.ToStringAdress().CapFirst() + " has killed " + (attacker == this ? "itself" : ToStringAdress()) + ".";
        }

        public void ShowDeathMessage(string msg)
        {
            Level.Server.User.Log.Add(msg);
        }




        // Status Effect Handlers:
        public virtual bool IsImmuneToStatusEffect(int statusID, string name) => this.IsImmune(statusID, name);

        /// <summary>
        /// Not currently used.
        /// </summary>
        public virtual bool IsVulnerableToStatusEffect(int statusID, string name) => this.IsVulnerable(statusID, name);

        /// <summary>
        /// Adds a status effect to this entity, Unless immume.
        /// Overrides similar already present effect if level or new time remaining is greater than previous.
        /// </summary>
        public bool AddStatusEffect(StatusEffect effect, bool allowRenew = false)
        {
            if (effect == null) return false;
            
            // Don't add if is immune
            if (IsImmuneToStatusEffect(effect.GetID(), effect.VariantName))
                return false;

            if (effect.GetID() == StatusEffectID.Poison && HasStatusEffect(StatusEffectID.Antidote)
                || effect.GetID() == StatusEffectID.Burning && HasStatusEffect(StatusEffectID.Antifire))
                return false;

            // Remove "opposite" status effects:
            if (effect.GetID() == StatusEffectID.Burning)
                RemoveStatusEffects(s => s.GetID() == StatusEffectID.Frostbite || s.GetID() == StatusEffectID.Frozen && s.Name == "frozen");
            else if (effect.GetID() == StatusEffectID.Frozen && effect.Name == "frozen" || effect.GetID() == StatusEffectID.Frostbite
                 || effect.GetID() == StatusEffectID.Antifire)
                RemoveStatusEffects(s => s.GetID() == StatusEffectID.Burning);
            else if (effect.GetID() == StatusEffectID.Antidote)
                RemoveStatusEffects(s => s.GetID() == StatusEffectID.Poison);

            // Try to replace any existing if Level is greater or the same and Time is greater than previous
            var existing = StatusEffects.Find(s => s.GetID() == effect.GetID());
            if (existing != null)
            {
                if (effect.Level > existing.Level || allowRenew && effect.Level == existing.Level && effect.Time > existing.Time)
                {
                    StatusEffects.Insert(StatusEffects.IndexOf(existing), effect);
                    StatusEffects.Remove(existing);
                    return true;
                }
                return false;
            }

            StatusEffects.Add(effect);

            return true;
        }
        public bool HasStatusEffect(int id)
        {
            return StatusEffects.Find(s => s?.GetID() == id) != null ? true : false;
        }
        public bool TryGetStatusEffect(int id, out StatusEffect effect)
        {
            effect = null;
            foreach (var se in StatusEffects)
            {
                if (se.GetID() == id)
                {
                    effect = se;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes status effect & returns whether it was removed/existed.
        /// </summary>
        public bool RemoveStatusEffect(StatusEffect effect)
        {
            if (StatusEffects.Remove(effect))
                return true;

            var existing = StatusEffects.Find(s => s.GetType() == effect.GetType());
            return StatusEffects.Remove(existing);
        }

        public bool RemoveStatusEffect(int id)
        {
            return StatusEffects.Remove(StatusEffects.Find(s => s.GetID() == id));
        }

        /// <summary>
        /// Removes all statuseffects matching the given predicate
        /// </summary>
        /// <param name="pred"></param>
        public void RemoveStatusEffects(Func<StatusEffect, bool> pred)
        {
            if (pred == null) return;
            StatusEffects = StatusEffects.Where(s => !pred.Invoke(s)).ToList();
        }




        /// <summary>
        /// Checks whether block is see-through for this entity.
        /// </summary>
        public virtual bool CanSeeThroughBlock(Block block) =>  !block.Data.BlocksLight;

        /// <summary>
        /// Checks whether block is aimable-through for this entity.
        /// </summary>
        public virtual bool CanAimOverBlock(Block block) => block.Data.IsAimable;



        public IEnumerable<Vector> SightSquares;
        /// <summary>
        /// Returns whether the other instance is visible to this instance.
        /// -> At least one tile must be both in LOS as well as have a light level of greater than zero.)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanSee(Instance other)
        {
            if (other == this || IsCollisionWith(other))
                return true;
            
            var calculator = new VisibilityCalculator(this);
            calculator.IsInstanceVisible(other);

            return other.GetTiles().Any(t => calculator.VisibleTiles.Contains(t) && Level.GetTileLightness(t) > 0);
        }

        public bool CanSee(Vector target)
        {
            var calculator = new VisibilityCalculator(this);
            calculator.IsTileVisible(target);

            return calculator.VisibleTiles.Contains(target) && Level.GetTileLightness(target) > 0;
        }

        /// <summary>
        /// Checks whether the specified instance has a particular tile under the following condtions:
        ///  1. In visible light (i.e. on a tile with lightness > 0),
        ///  2. In sight line (i.e. not obstructed by any vision-blocking objects).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanAimAt(Instance other)
        {
            if (other == this) return true;

            var calculator = new VisibilityCalculator(this);
            calculator.IsInstanceVisible(other);

            return other.GetTiles().Any(t => calculator.VisibleTiles.Contains(t) && Level.GetTileLightness(t) > 0);
        }


        //private bool CheckSightLine(int xstart, int ystart, int xend, int yend)
        //{
        //    var start = new Vector(xstart, ystart);
        //    var end = new Vector(xend, yend);

        //    var ray = start.CastRay(end, true, true);

        //    SightSquares = ray;

        //    //check marked squares for see through
        //    int tile;
        //    foreach (var point in ray)
        //    {
        //        tile = Level.TileMap[point.X, point.Y].Floor;
        //        if (!CanSeeThroughTile(tile))
        //        {
        //            return false;
        //        }
        //    }


        //    return true;
        //}




        public override void Draw(Vector2 pos, float lightness)
        {
            //var size = new Vector2(GetW(), GetH()) * GameClient.GRID_SIZE; // Real pixel size of this entity

            //Display.DrawRect(drawPos - size / 2, size, Color.LightBlue);
            //var offset = -size / 2;
            //float scale = GetW();
            //if (StatusEffects.Exists(effect => effect.VariantName == "frozen"))
            //    Display.DrawTexture(pos + offset, Assets.Ice, scale);

            base.Draw(pos, lightness);
        }
    }
}
