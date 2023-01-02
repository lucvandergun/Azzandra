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
    public abstract class Enemy : NPC
    {
        // == Saved attributes == \\
        public int HitTimer { get; protected set; } = 10;
        public bool IsFleeing { get; protected set; } = false;


        // == Properties == \\
        public virtual int GetSightRange() => 8;        // The distance the enemy can see
        protected virtual bool IsAgressive() => true;   // Whether the enemy will pick a fight with the player
        protected virtual bool FightBack() => true;     // Whether the enemy will fight back when attacked
        //public virtual bool CanChase() => false;        // Whether the enemy will chase a target outside its wander range
        
        public virtual int DetectRange => 5;            // The range at which they will target an instance: measured from self.
        public virtual int AggressiveRange => 2;        // The range up to which they will always be aggressive towards a target (additionally to wander range, from base pos)
        public virtual int LoseTargetTime => 10;        // The time needed to pass after last being hit by a target in order to consider returning to the wander area.
        public virtual bool ReturnHome() => HitTimer >= LoseTargetTime && LoseTargetTime != -1 && !IsInRangeFromPoint(BasePosition, WanderRange + AggressiveRange);
        public virtual bool CanFlee() => true;
        public virtual float FleeHpThreshold => 0.25f;  // The Hp at which the enemy will flee
        public virtual bool FleesIfCannotAttackBack() => true;



        // == Combat attributes == \\
        // Stats & Attacks are loaded from Data upon initialization
        public EntityStats Stats;
        public TemplateAttack[] Attacks;
        public List<ActionTemplate> Spells = new List<ActionTemplate>();
        public virtual float SpellChance => 0.25f;

        //public override int GetFullHp()
        //{
        //    // Read health from direct stats, else 
        //    return Stats?.Health ?? 1;
        //}


        
        public override float GetEvd() => base.GetEvd() * Stats.Evade;
        public override float GetPar() => base.GetPar() * Stats.Parry;
        public override float GetBlk() => base.GetBlk() * Stats.Block;
        public override float GetRes() => base.GetRes() * Stats.Resistance;
        public override float GetArm() => base.GetArm() * Stats.Armour;



        public Enemy(int x, int y) : base(x, y)
        {
            SetupStats();
            SetupActionPotentials();
            FullHp = Data.GetEnemyData(InstanceID.GetTypeID2(this)).Stats.Health;
            Hp = GetFullHp();
        }

        public override void Load(byte[] bytes, ref int pos)
        {
            SetupStats();
            SetupActionPotentials();

            HitTimer = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            IsFleeing = BitConverter.ToBoolean(bytes, pos);
            pos += 1;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[5];
            
            bytes.Insert(0, BitConverter.GetBytes(HitTimer));
            bytes.Insert(4, BitConverter.GetBytes(IsFleeing));

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        protected void SetupStats()
        {
            // Load the enemy stats from the database
            var data = Data.GetEnemyData(InstanceID.GetTypeID2(this));
            Initiative = data.Initiative;
            Stats = data.Stats;
            Attacks = data.Attacks;
        }

        /// <summary>
        /// Used to create a list of spell actions to be done. Can be overridden to add e.g. spells.
        /// </summary>
        protected virtual void SetupActionPotentials() { }

        public override void TurnStart()
        {
            base.TurnStart();

            if (Target != null && HitTimer < 20) HitTimer++;
        }

        public override void Turn()
        {
            Action = DetermineAction();
            base.Turn();
        }

        protected virtual EntityAction DetermineAction()
        {
            //var player = Level.Server.User.Player;
            //if (player != null)
            //{
            //    var map = new DijkstraMap(Level, this, new List<Instance>() { player });
            //    map.CreateMap2();
            //    map.IterateOverMap();
            //    map.ToIntMatrix();
            //    var step = map.GetStep();

            //    //var avMap = new AvoidanceMap(Level, this);
            //    //avMap.CreateMap();
            //    //map.CombineWith(avMap);
            //    //var step = map.GetStep();

            //    Debug.WriteLine(map.Matrix.Stringify());
            //    Debug.WriteLine("loc: " + Position + ", step: " + step);
            //    return new ActionMove(this, step);
            //}
            
            
            // Check whether to lose target:
            if (Target != null)
            {
                if (!IsTargetStillValid() || ReturnHome())
                {
                    //if (returnHome && Target.Instance is Player player)
                    //    player.User.ShowMessage(ToStringAdress().CapFirst() + " seems to have stopped attacking you." );

                    Level.Server.User.ThrowDebug(ToString() + " has lost target: [" + Target + "].");
                    Target = null;
                    Action = null; // Remove any action aimed at the target
                    NextAction = null;
                }
            }

            // Find target if desired:
            if (Target == null && IsAgressive() && IsInRangeFromPoint(BasePosition, WanderRange))
            {
                var newTarget = FindTarget();
                if (newTarget != null)
                {
                    Target = new InstRef(newTarget);
                    Action = null;
                    NextAction = null;

                    // Player is spotted message:
                    //if (newTarget is Player player)
                    //    player.User.ShowMessage("<yellow>" + ToStringAdress().CapFirst() + " has spotted you.");

                    Level.Server.User.ThrowDebug(ID + " has found target: " + Target + ", name: " + newTarget.Name);
                }
            }

            // Set "IsFleeing":
            if (!IsFleeing)
            {
                if (Target != null && Hp <= FullHp * FleeHpThreshold)
                    IsFleeing = true;
            }

            if (IsFleeing)
            {
                if (Target == null ||
                    CanAffectivelyReachTarget(Target.Instance) && Hp > FullHp * FleeHpThreshold ||
                    TileDistanceTo(Target.Instance) >= 10)
                {
                    IsFleeing = false;
                }
            }

            // Decide upon behaviour:
            if (Target != null)
            {
                if (IsFleeing)
                    return DetermineFleeAction();
                else
                    return DetermineAggressiveAction();
            }
            else
            {
                return DetermineRegularAction();
            }
        }


        /// <summary>
        /// This method should be able to assume that enemy has a valid target.
        /// </summary>
        public virtual EntityAction DetermineFleeAction()
        {
            if (BasePosition != null)
                return new ActionPath(this, BasePosition.Value, false);
            else
                return new ActionFlee(this, Target.Instance);
        }

        /// <summary>
        /// This method should be able to assume that enemy has a valid target.
        /// </summary>
        public virtual EntityAction DetermineAggressiveAction()
        {
            ////// Keep any current action
            ////if (Action != null)
            ////    return Action;

            // Decide upon attack type to use:
            var template = ChooseActionTemplate();
            return template?.ToAction(this);
        }

        /// <summary>
        /// Returns a usable (best) attack type to attack the target. (Is maily based on range.)
        /// </summary>
        /// <param name="target">The target to try to attack.</param>
        /// <returns>A usable attack type. Returns null if no target or if none are suitable.</returns>
        protected virtual ActionTemplate ChooseActionTemplate()
        {
            // Try, by chance, to perform a spell first
            if (Util.Random.NextDouble() <= SpellChance)
            {
                var spellPotentials = Spells.Where(a => a.CanBePerformed(this) && (!(a is TemplateAffect aff) || aff.IsInRange(TileDistanceTo(Target.Instance)))).ToList();
                if (spellPotentials.Count > 0)
                {
                    return spellPotentials.PickRandom();
                }
            }
            
            // Try to find an attack that can already be performed (in-range)
            var potentials = Attacks.Where(a => a.IsInRange(TileDistanceTo(Target.Instance))).ToList();
            if (potentials.Count > 0)
            {
                return potentials.PickRandom();
            }

            // Otherwise, return the attack that is closest in-range
            var maxRange = Attacks.Max(a => a.Range);
            return Attacks.FirstOrDefault(a => a.Range == maxRange);
        }

        public virtual EntityAction CreateActionForAffect(Instance target, Affect affect)
        {
            if (target == this)
                return new ActionAffect(this, this, affect);

            var entity = target as Entity;
            if (entity == null && affect is Attack)
                return null;
            
            var dist = DistanceTo(target);

            // Move from under target:
            if (dist == Vector.Zero)
                return new ActionMove(this, Dir.Random.ToVector());

            // Try to attack directly:
            if (CanAffect(target, affect))
            {
                if (AttackTimer >= affect.Speed)
                    return new ActionAffect(this, target, affect);
                else
                    return null;
            }

            // Check whether enemy can move at all:
            if (!CanMove()) return null;


            // Try to leap at target if within one turn's movement:
            if (affect is Attack attack && attack.Style == Style.Melee && AttackTimer >= affect.Speed &&
                dist.Absolute() - new Vector(affect.Range) <= new Vector(GetMovementSpeed()))
            {
                return new ActionLeapAttack2(this, entity, (Attack)affect);
            }


            //var step = GetStepTowards(target);
            //var movementOptions = new Vector[] { step, new Vector(step.X, 0), new Vector(0, step.Y) };
            //foreach (var option in movementOptions.Where(o => !o.IsNull()))
            //{
            //    // Try to leap at target if within one turn's movement:
            //    if (affect is Attack attack && attack.Style == Style.Melee && AttackTimer >= affect.Speed)
            //    {
            //        if (dist.Absolute() - new Vector(affect.Range) <= new Vector(GetMovementSpeed()))
            //            return new ActionLeapAttack(this, entity, option, (Attack)affect);
            //    }
            //}


            // TODO: move towards closest tile that meets specs: i.e. tiles in attack range!
            // Make sure not to needlessly override old PathTarget actions.
            //if (Action == null || !(Action is ActionPathTarget))
            //else return Action;

            // Just move towards the target:
            return new ActionPathTarget(this, target, false);
        }


        protected bool CanChaseToPos(Vector pos)
        {
            var reg = GetRegionAroundBasePos(WanderRange + AggressiveRange);
            return reg == null || pos.IsInRegion(reg);
        }


        /// <summary>
        /// Returns a viable target Combatant, such a target must be:
        ///  1. Desired.
        ///  2. In a range from the base point.
        ///  3. Visible.
        /// </summary>
        protected virtual Entity FindTarget()
        {
            foreach (var inst in Level.ActiveInstances)
            {
                if (!(inst is Entity cb))
                    continue;

                bool isInAggressiveRegion = inst.IsInRangeFromPoint(BasePosition, WanderRange + AggressiveRange);
                bool isInDetectRange = DistanceTo(cb).ChebyshevLength() <= DetectRange;

                if (IsATarget(cb)&& cb.Hp > 0 && isInAggressiveRegion && isInDetectRange && CanSee(inst))
                {
                    HitTimer = 0;
                    return cb;
                }

                //if (IsATarget(cb) && inst.IsInRangeFromPoint(BasePosition, range) && CanSee(inst))
                //{
                //    HitTimer = 0;
                //    return cb;
                //}
            }

            return null;
        }

        /// <summary>
        /// Returns true if supplied entity is a target of this entity.
        /// </summary>
        protected virtual bool IsATarget(Entity inst)
        {
            if (inst.IsAttackable() && inst is Player)
                return true;

            return false;
        }

        /// <summary>
        /// Returns supplied target if its still a valid target:
        /// Target exists, has health > 0, is attackable and 
        /// </summary>
        protected virtual bool IsTargetStillValid()
        {
            var inst = Target?.Combatant;
            
            // Check instance actually exists and is not dead.
            if (inst == null || inst.Hp <= 0)
                return false;
            
            // Check instance is attackable and visible
            if (!inst.IsAttackable() || !IsInRange(inst, GetSightRange()) || !CanSee(inst))
                return false;

            return true;
        }


        // == Combat == \\

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            // Start fighting back, if: not yet a target OR current target has not attacked for a while.
            bool canSetTarget = Target == null || HitTimer >= LoseTargetTime / 2;
            if (FightBack() && attacker != this && attacker != null && canSetTarget)
            {
                Target = new InstRef(attacker);
            }

            // Flee if cannot attack back:
            if (FleesIfCannotAttackBack() && attacker == Target.Instance)
            {
                if (!CanAffectivelyReachTarget(Target.Instance))
                {
                    Level.Server.ThrowDebug(ToStringAdress() + " was unable to chase " + attacker.ToStringAdress() + ", lost target.");
                    IsFleeing = true;
                }
            }
                
            // Reset hit timer
            if (Target != null && attacker == Target.Combatant)
                HitTimer = 0;

            return base.GetAffected(attacker, affect);
        }

        protected bool CanAffectivelyReachTarget(Instance inst)
        {
            var maxAttackRange = Attacks.Max(a => a.Range);
            var maxRangeAttack = Attacks.FirstOrDefault(a => a.Range == maxAttackRange).ToAffect(Level.Server);

            if (CanAffect(inst, maxRangeAttack))
                return true;

            var newPath = new ActionPathTarget(this, inst, true);
            if (newPath.Path.Length >= 1)
                return true;

            return false;
        }


        protected override void ApplyDeathEffects()
        {
            DropItemsOnDeath();
        }

        protected virtual void DropItemsOnDeath() { }


        // == Rendering == \\
        public override Symbol GetSymbol() => new Symbol(Name.First(), Color.Yellow);

        public override void DrawView(SpriteBatch sb, Vector2 viewOffset, Server server, float lightness)
        {
            base.DrawView(sb, viewOffset, server, lightness);

            // Outline Target Instance
            if (server.GameClient.IsDevMode && server.GameClient.IsDebug && Target != null)
            {
                var target = Target.Combatant;
                if (target != null)
                {
                    var size = target.Size.ToFloat() * GameClient.GRID_SIZE;
                    var drawPos = target.CalculateRealPos(server) + viewOffset - size / 2;
                    var rect = Display.MakeRectangle(drawPos, size);
                    var color = GetSymbol().Color;

                    Display.DrawOutline(rect, color);
                }
                
            }
        }
    }
}
