using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Player : Entity
    {
        protected override int VisionRange => 24;
        public bool ReQueueActions() => Level?.Server.GameClient.Engine.Settings.ReQueueing ?? false;

        public User User { get; private set; }
        //public override int GetFullHp()
        //{
        //    if (User != null)
        //    {
        //        int calcedHp = User.Stats.GetLevel(SkillID.Vitality) * 4 + 20;
        //        return (int)(calcedHp * User.Class.HealthMod);
        //    }

        //    return 20;
        //}

        public int BaseHp => (int)(20f * User?.Class.HealthMod ?? 1);
        public int HpPerLevel => (int)(4f * User?.Class.HealthMod ?? 1);

        public int BaseSp => 10;
        public int SpPerLevel => 2;
        public int FullSp { get; set; } = 10;
        protected int _sp;
        public int Sp { get => _sp; set => _sp = Util.Boundarize(value, 0, GetFullSp()); }
        public virtual int GetFullSp() => FullSp;

        public const int FullHunger = 5;
        protected int _hunger;
        public int Hunger { get => _hunger; set => _hunger = Util.Boundarize(value, 0, GetFullHunger()); }
        public virtual int GetFullHunger() => FullHunger;

        public int RegenTimer = 0;
        public readonly int RegenDelay = 5;


        public override int GetMovementSpeed() => 1;
        public override bool IsAttackable() => !User.IsCheatMode && base.IsAttackable();
        public override bool SlidesOnIce() => !User.IsCheatMode && base.SlidesOnIce();
        public override bool CanBeDestroyed() => !User.IsCheatMode && base.CanBeDestroyed();


        // === Combat Property Getters === \\
        public float GetAcc(Style style) //MidpointRounding.AwayFromZero
        {
            float acc;
            switch (style)
            {
                default: return 0;;
                case Style.Melee:
                    acc = (GetLevel(SkillID.Attack) + User.Equipment.Accuracy) / 2f * User.Class.MeleeAccuracyMod;
                    break;
                case Style.Ranged:
                    acc = (GetLevel(SkillID.Ranged) + User.Equipment.Accuracy) / 2f * User.Class.RangedAccuracyMod;
                    break;
                case Style.Magic:
                    acc = (GetLevel(SkillID.Magic) + User.Equipment.Accuracy) / 2f * User.Class.MagicAccuracyMod;
                    break;
            }

            return AccuracyMod * acc;
        }
        public float GetDmg(Style style)
        {
            float dmg;
            switch (style)
            {
                default: return 0;
                case Style.Melee:
                    dmg = (GetLevel(SkillID.Attack) + User.Equipment.Damage) / 2f * User.Class.MeleeDamageMod;
                    break;
                case Style.Ranged:
                    var ammoStr = User.Equipment.GetAmmunitionStrength();
                    dmg = (GetLevel(SkillID.Ranged) + User.Equipment.Damage + ammoStr) / 2f * User.Class.RangedDamageMod;
                    break;
                case Style.Magic:
                    dmg = (GetLevel(SkillID.Magic) + User.Equipment.Damage) / 2f * User.Class.MagicDamageMod;
                    break;
            }

            return DamageMod * dmg;
        }
        public float GetSpc()
        {
            return SpellcastMod * (GetLevel(SkillID.Magic) + User.Equipment.Spellcast) / 2f * User.Class.SpellcastMod;
        }
        public override float GetPar()
        {
            if (!User.Equipment.CanParry())
                return 0;

            return base.GetPar() * (GetLevel(SkillID.Defense) + User.Equipment.Parry) / 2f * User.Class.ParryMod;
        }
        public override float GetBlk()
        {
            if (!User.Equipment.CanBlock())
                return 0;

            return base.GetBlk() * (GetLevel(SkillID.Defense) + User.Equipment.Block) / 2f * User.Class.BlockMod;
        }
        public override float GetEvd()
        {
            return base.GetEvd() * (GetLevel(SkillID.Evade) + User.Equipment.Evade) / 2f * User.Class.EvadeMod;
        }
        public override float GetRes()
        {
            return base.GetRes() * (GetLevel(SkillID.Magic) + User.Equipment.Resistance) / 2f * User.Class.ResistanceMod;
        }
        public override float GetArm()
        {
            return base.GetArm() * User.Equipment.Armour;
        }


        public int GetAttackRange(Style style)
        {
            return User.Equipment.AttackRange;
        }

        public int GetAttackSpeed(Style style)
        {
            return User.Equipment.AttackSpeed;
        }

        /// <summary>
        /// Returns the user's level with the corresponding class boosts.
        /// </summary>
        /// <param name="id"></param>
        public float GetLevel(int id)
        {
            return User.Stats.GetLevel(id);
        }


        public Player(int x, int y, User user) : base(x, y)
        {
            User = user;

            FullHp = BaseHp + HpPerLevel * User.Stats.GetLevel(SkillID.Vitality);
            Hp = GetFullHp();

            FullSp = BaseSp + SpPerLevel * User.Stats.GetLevel(SkillID.Magic);
            Sp = GetFullSp();

            Hunger = 2;
            RegenTimer = RegenDelay;
        }

        public override void Load(byte[] bytes, ref int pos)
        {
            _sp = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            FullSp = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Hunger = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[12];

            bytes.Insert(0, BitConverter.GetBytes(Sp));
            bytes.Insert(4, BitConverter.GetBytes(FullSp));
            bytes.Insert(8, BitConverter.GetBytes(Hunger));

            return bytes.Concat(base.ToBytes()).ToArray();
        }

        public override void TurnStart()
        {
            base.TurnStart();

            //Remove target if no longer visible:
            if (Target?.Combatant?.Hp <= 0)
                Target = null;

            // Regeneration
            if (Hunger < GetFullHunger())
            {
                if (TryGetStatusEffect(StatusEffectID.Starving, out var effect))
                {
                    RemoveStatusEffect(effect);
                    User.ShowMessage("You are no longer starving.");
                }
            }
            else if(!HasStatusEffect(StatusEffectID.Starving))
            {
                AddStatusEffect(new StatusEffects.Starving());
                User.ShowMessage("<red>You are starving!");
            }

            //if (RegenTimer == 0)
            //{
            //    if (Hunger > 0) Regenerate();
            //    RegenTimer = RegenDelay;
            //}
            //if (RegenTimer > 0) RegenTimer--;
        }

        /// <summary>
        /// Consume 1 food from the counter to restore some hp and sp
        /// </summary>
        public void Regenerate()
        {
            if (Hunger <= 0) return;

            // default 5% heal at full food, 15% at 1 food?
            // (8 + 2)% of full hp + 1 for each vitality level
            float hpAmt = GetFullHp() * (0.08f + 0.02f * GetLevel(SkillID.Vitality));
            Heal((int)hpAmt);

            float spAmt = GetFullSp() * 0.1f;
            Sp += (int)spAmt;

            // Decrease Food by 1
            Hunger--;
        }

        /// <summary>
        /// Rest method to restore hp and sp. Increases hunger by 1.
        /// </summary>
        public void Rest()
        {
            int newHp = GetFullHp();
            if (HasStatusEffect(StatusEffectID.Fatigue)) newHp /= 2;
            Hp = Math.Max(Hp, newHp);
            Sp = GetFullSp();
            Hunger++;

            var effects = StatusEffects.CreateCopy();
            foreach (var effect in effects)
            {
                if (effect?.IsPermanent ?? true)
                    RemoveStatusEffect(effect);
            }
        }

        protected override void ApplyDeathEffects()
        {
            // Open up victory interface:
            User.GameOver();

            // User.ShowMessage("<red>You have died.");
            //User.Respawn(Level);
        }

        public override string GetDeathMessage(Entity attacker)
        {
            return attacker is Player
                ? "<red>You have been slain by yourself. Oh what has it come to..."
                : "<red>You have been killed by " + attacker.ToStringAdress() + "!";
        }


        public override Symbol GetSymbol() { return new Symbol('p', Color.Lime); }


        public override bool CanMove()
        {
            return User.IsCheatMode || base.CanMove();
        }

        public override bool CanMoveUnobstructed(int x, int y, int dx, int dy, bool incorporateEntities = true)
        {
            return User.IsCheatMode || base.CanMoveUnobstructed(x, y, dx, dy, incorporateEntities);
        }

        public override void OnOtherInstanceCollision(Instance inst)
        {
            if (inst is GroundItem grit && grit.Item is Items.Ammunition ammo)
            {
                if (User.Inventory.HasItem(i => i.ID == ammo.ID))
                {
                    User.Inventory.AddItem(ammo);
                    grit.Destroy();
                }
            }

            base.OnOtherInstanceCollision(inst);
        }

        public override Affect Affect(Instance target, Affect affect)
        {
            //var targetWasDead = (target as Entity)?.Hp > 0;
            affect = base.Affect(target, affect);

            if (affect is Attack attack)
            {
                // Take ammunition out of inventory
                if (attack.Style == Style.Ranged)
                {
                    if (User.Equipment.Items[0] is Items.RangedWeapon rangedWeapon)
                    {
                        var ammo = User.Inventory.GetAmmunitionByType(rangedWeapon.AmmunitionType);
                        if (ammo != null)
                        {
                            ammo.Quantity -= 1;

                            // Create arrow on ground if missed.
                            if (attack.HitType == HitType.Evaded)
                            {
                                var oldAmmo = (GroundItem)Level.ActiveInstances.FirstOrDefault(i => i is GroundItem g && g.Position == target.Position && g.Item.ID == ammo.ID);
                                if (oldAmmo == null)
                                    Level.CreateInstance(new GroundItem(target.X, target.Y, Item.Create(ammo.ID, 1)));
                                else
                                    oldAmmo.Item.Quantity++;
                            }
                        }
                    }
                }
                // Use staff durability
                else if (attack.Style == Style.Magic)
                {
                    if (User.Equipment.Items[0] is Items.MagicWeapon magicWeapon)
                    {
                        magicWeapon.DecreaseDurability(1);
                    }
                }
            }

            // On-kill gains:
            //if (targetWasDead && target is Entity entity && entity.Hp <= 0)
            //    Sp += entity.GetSpRestore();

            return affect;
        }

        public override Affect GetAffected(Entity attacker, Affect affect)
        {
            if (User.IsCheatMode && attacker != this)
            {
                affect.Fail(null);
                return affect;
            }

            // Stop resting:
            if (Action is ActionRest)
            {
                Action = null;
                User.ShowMessage("<rose>Your resting was interrupted!");
            }

            return base.GetAffected(attacker, affect);
        }

        public override int GetHit(Style style, int dmg)
        {
            if (!User.IsCheatMode)
                return base.GetHit(style, dmg);
            return 0;
        }



        public override bool CanAffect(Instance target, Affect affect)
        {
            //if (target == this && affect is Attack)
            //{
            //    User.Log.Add("<orange>You shouldn't be attacking yourself, you fool!");
            //    return false;
            //}

            if (target is Entity entity && entity.Hp <= 0)
            {
                User.ShowMessage("That target is already dead.");
                return false;
            }

            // Check for required ammo if style is ranged
            if (affect is Attack attack && attack.Style == Style.Ranged)
            {
                var weapon = User.Equipment.Items[0];
                if (weapon is Items.RangedWeapon rangedWeapon)
                {
                    if (!User.Inventory.HasAmmunitionType(rangedWeapon.AmmunitionType))
                    {
                        User.Log.Add("You don't have any " + rangedWeapon.AmmunitionType.ToString().ToLower() + "s to shoot.");
                        return false;
                    }
                }
            }

            if (this == target)
                return true;

            var type = affect is Attack ? "attack" : "cast a spell on"; // "tame"

            //if (!target.IsAttackable())
            //{
            //    var str = affect is Attack ? "attacked" : "affected by spells";
            //    User.Log.Add(target.ToStringAdress().CapitalizeFirst() + " cannot be " + str + ".");
            //    return false;
            //}
            if (IsInstanceSolidToThis(target) && IsCollisionWith(target))
            {
                User.Log.Add("You cannot properly " + type  + " " + target.ToStringAdress() + " while you are on top of it.");
                return false;
            }

            if (!IsInAttackRange(target, affect.Range))
            {
                User.Log.Add(target.ToStringAdress().CapFirst() + " is too far away for you to " + type + ".");
                return false;
            }

            if (!CanAimAt(target))
            {
                User.Log.Add("There is something obstructing you to " + type + " " + target.ToStringAdress() + ".");
                return false;
            } 

            return true;
        }
    }



    
}
