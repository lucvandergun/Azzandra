using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra.SpellEffects
{
    public class FireCage : SpellEffect
    {
        public FireCage() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var nw = target.Position;
            var se = target.Position + target.Size;
            int dist = 2;
            var level = target.Level;

            for (int i, j = nw.Y - dist; j < se.Y + dist; j++)
            {
                for (i = nw.X - dist; i < se.X + dist; i++)
                {
                    if (i == nw.X - dist || i == se.X + dist - 1 || j == nw.Y - dist || j == se.Y + dist - 1)
                    {
                        var fire = new GroundFire(i, j);
                        if (level.CanCreateInstance(fire))
                        {
                            level.CreateInstance(fire);
                            attacker.Children.Add(new InstRef(fire));
                        }
                    }
                }
            }

            var specs = GetMsgAdresses(attacker, target);
            attacker.Level.Server.User.ShowMessage("<orange>" + specs.Item1.CapFirst() + " " + GetVerb(attacker, "surround") + " " + specs.Item2 + " with a cage of fire.");
        }
    }

    public class MagneticPull : SpellEffect
    {
        public MagneticPull() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            if (!(target is Entity cbtarget))
                return;
            
            // Move target to touch attacker, if touching: stun!
            var dist = cbtarget.DistanceTo(attacker);
            cbtarget.Move(dist);

            var specs = GetMsgAdresses(attacker, target);
            attacker.Level.Server.User.ShowMessage("<dkred>" + specs.Item1.CapFirst() + " pulls " + specs.Item2 + " closer.");

            if (cbtarget.IsTouching(attacker))
            {
                cbtarget.AddStatusEffect(new StatusEffects.Stunned(1, 8));

                var specs2 = GetMsgAdressesHave(target, target);
                attacker.Level.Server.User.ShowMessage("<yellow>" + specs2.Item1.CapFirst() + " been stunned!");
            }
        }
    }

    public class SummonFiend : SpellEffect
    {
        public SummonFiend() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var fiend = new Cockatrice(attacker.X, attacker.Y);
            fiend.Parent = new InstRef(attacker);
            //fiend.Target = new InstRef(target);
            attacker.Level.CreateInstance(fiend);
            attacker.Children.Add(new InstRef(fiend));

            var specs = GetMsgAdresses(attacker, target);
            attacker.Level.Server.User.ShowMessage("<orange>" + specs.Item1.CapFirst() + " summons a fiend.");
        }
    }

    public class ShadowCloud : SpellEffect
    {
        public ShadowCloud() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            if (!(target is Entity cbtarget))
                return;

            var cloud = new Azzandra.ShadowCloud(attacker.X, attacker.Y, attacker, cbtarget);
            attacker.Level.CreateInstance(cloud);
            attacker.Children.Add(new InstRef(cloud));

            if (target is Player)
            {
                var specs = GetMsgAdresses(attacker, target);
                attacker.Level.Server.User.ShowMessage("<purple>A cloud has formed before " + specs.Item2 + "!");
            }
        }
    }

    public class Revive : SpellEffect
    {
        public Revive() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            if (!(target is Grave grave))
                return;
            
            // Set grave to empty
            grave.IsEmpty = true;

            // Spawn skele
            var inst = new Skeleton(grave.X, grave.Y);
            attacker.Level.CreateInstance(inst);
            attacker.Children.Add(new InstRef(inst));

            var specs = GetMsgAdresses(attacker, target);
            attacker.Level.Server.User.ShowMessage("<dkred>" + specs.Item1.CapFirst() + " " + GetVerb(attacker, "rise") + " something from the grave!");
        }
    }

    public class Howl : SpellEffectAcute
    {
        public Howl() { }

        public override void Apply(Entity attacker)
        {
            foreach (var c in attacker.Children)
            {
                if (!(c.Instance is Wolf wolf))
                    continue;

                wolf.Target = new InstRef(attacker.Target.Instance);
            }

            var specs = GetMsgAdresses(attacker, attacker);
            attacker.Level.Server.User.ShowMessage("<medblue>" + specs.Item1 + " " + GetVerb(attacker, "howl") + " to the nearby wolves!");
        }
    }

    public class Web : SpellEffect
    {
        public Web() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            attacker.Level.CreateInstance(new SpellProjectile(attacker, target, TileDisplay.Get(BlockID.Cobweb).Symbol));

            var specs = GetMsgAdresses(attacker, target);

            // Freeze target for 1 turn
            if (target is Entity entity && !entity.IsTypeOf(EntityType.Spider))
            {
                var effect = new StatusEffects.Frozen(1, 1, "wrapped");
                entity.AddStatusEffect(effect);
                attacker.Level.Server.User.ShowMessage(specs.Item1 + " " + effect.VariantName + " " + specs.Item2 + " in a web!");
            }

            // Create web tile
            foreach (var tile in target.GetTiles())
                target.Level.SetObject(tile, new Block(BlockID.Cobweb), false);
        }
    }

    public abstract class StatusEffectSpell : SpellEffect
    {
        public StatusEffectSpell() { }

        public abstract StatusEffect GetStatusEffect();

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var specs = GetMsgAdresses(attacker, target);
            var effect = GetStatusEffect();
            if (!(target is Entity entity))
            {
                attacker.Level.Server.User.ShowMessage(((target is Player) ? "you" : target.ToStringAdress()).CapFirst() + " " + GetVerb(target, "are", "is") + " is not affected by being " + effect.VariantName + ".");
                return;
            }
            else if (!entity.AddStatusEffect(effect, true))
            {
                attacker.Level.Server.User.ShowMessage(((target is Player) ? "you" : target.ToStringAdress()).CapFirst() + " " + GetVerb(target, "are", "is") + " immune to being " + effect.VariantName + ".");
                return;
            }

            attacker.Level.Server.User.ShowMessage("<medblue>" + specs.Item1 + " " + GetVerb(attacker, "cast") + " " + GetType().Name.ToLower() + " on " + specs.Item2 + "!");
        }
    }

    public class Frostbite : StatusEffectSpell
    {
        public Frostbite() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Frostbite(2, 13);
    }
    public class Freeze : StatusEffectSpell
    {
        public Freeze() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Frozen(1, 8);
    }
    public class Ensnare : StatusEffectSpell
    {
        public Ensnare() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Frozen(1, 16, "ensnared");
    }
    public class Blind : StatusEffectSpell
    {
        public Blind() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Blind(1, 10);
    }
    public class Weaken : StatusEffectSpell
    {
        public Weaken() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Weak(1, 20);
    }

    public class Disorient : StatusEffectSpell
    {
        public Disorient() { }
        public override StatusEffect GetStatusEffect() => new StatusEffects.Disoriented(1, 6);
    }

    public class Deflect : SpellEffectAcute
    {
        public Deflect() { }
        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            var effect = new StatusEffects.Deflect(1, 1);
            attacker.AddStatusEffect(effect, true);
            attacker.Level.Server.User.ShowMessage("<medblue>" + specs.Item1 + " will deflect the next hit.");
        }
    }

    public class Shatter : SpellEffectAcute
    {
        public Shatter() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 3;
            var tiles = attacker.GetTiles();

            // Destroy rock tiles
            for (int i, j = -range; j < attacker.GetW() + range; j++)
            {
                for (i = -range; i < attacker.GetH() + range; i++)
                {
                    var tile = attacker.Position + new Vector(i, j);
                    var obj = attacker.Level.GetTile(tile).Object.ID;
                    if (tiles.Any(t => (t - tile).EuclidianLength() <= range) && (obj == BlockID.Rock || obj == BlockID.Crystal || obj == BlockID.Icicle) )
                        attacker.Level.SetObject(tile, new Block(BlockID.Void), true);
                }
            }

            attacker.Level.Server.User.ShowMessage("<medblue>" + specs.Item1 + " " + GetVerb(attacker, "shatter") + " all surrounding rocks!");

            // Destroy Rock entities
            foreach (Entity rock in attacker.Level.ActiveInstances.Where(i => i is Entity e && e.IsTypeOf(EntityType.Rock)))
            {
                if (attacker.DistanceTo(rock).ChebyshevLength() <= range && attacker.CanAffect(rock, new Attack(attacker.Level.Server, Style.Magic, 1, range, 1, 1)))
                {
                    rock.GetHit(Style.Other, rock.Hp);
                    attacker.Level.Server.User.ShowMessage(rock.GetDeathMessage(attacker));
                }
            }
        }
    }

    public class Telekinesis : SpellEffect
    {
        public Telekinesis() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var specs = GetMsgAdresses(attacker, target);

            var dist = attacker.DistanceTo(target);
            //if (!attacker.CanAffect(target, affect))
            //    return;

            // Transport items under caller
            if (target is GroundItem grit)
            {
                if (attacker is Player player && player.User.Inventory.CanAddItem(grit.Item))
                {
                    player.User.Inventory.AddItem(grit.Item);
                    grit.Destroy();
                    player.User.ShowMessage("<aqua>You move " + specs.Item2 + " to your inventory.");
                }
                else
                {
                    grit.Position -= dist;
                    grit.Animations.Add(new MovementAnimation(dist));
                    attacker.Level.Server.User.ShowMessage("<aqua>" + specs.Item1 + " " + GetVerb(attacker, "pull", "pulls") + " " + specs.Item2 + " nearer.");
                }
                return;
            }

            // Pull Entities:
            if (target is Entity entity)
            {
                entity.Move(new Vector(-dist.X, -dist.Y));
                entity.AddStatusEffect(new StatusEffects.Stunned(1, 1));
                attacker.Level.Server.User.ShowMessage("<aqua>" + specs.Item1 + " " + GetVerb(attacker, "pull", "pulls") + " " + specs.Item2 + " nearer.");
                return;
            }

            attacker.Level.Server.User.ShowMessage(((target is Player) ? "you" : target.ToStringAdress()).CapFirst() + " remained unaffected by the telekinesis.");
        }
    }

    public class Unveil : SpellEffect
    {
        public Unveil() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var specs = GetMsgAdresses(attacker, target);

            attacker.Level.Server.User.ShowMessage("" + specs.Item1 + " " + GetVerb(attacker, "unveil", "unveils") + " the object: <aqua>it is truly " + target.ToSecretStringArticle() + "!");
        }
    }

    public class Unlock : SpellEffect
    {
        public Unlock() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            var specs = GetMsgAdresses(attacker, target);

            if (target is LockedDoor door)
            {
                door.Unlock();
                attacker.Level.Server.User.ShowMessage("<cyan>" + specs.Item1 + " " + GetVerb(attacker, "unlock") + " " + specs.Item2 + ".");
            }
            else if (target is MultipleItemContainer || target is SingleItemContainer)
            {
                attacker.Level.Server.User.ShowMessage("" + specs.Item2 + " was already unlocked!");
            }
        }
    }

    public class Liberate : SpellEffectAcute
    {
        public Liberate() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            attacker.Level.Server.User.ShowMessage("<chiffon>" + specs.Item1 + " " + GetVerb(attacker, "liberate") + " " + specs.Item2 + " from any restraints.");

            var removeIDs = new int[] { StatusEffectID.Stunned, StatusEffectID.Frozen };

            attacker.StatusEffects.FilterOut(s => !removeIDs.Contains(s.GetID()));

            foreach (var pos in attacker.GetTiles())
            {
                if (attacker.Level.GetTile(pos).Object.ID == BlockID.Cobweb)
                    attacker.Level.SetObject(pos, new Block(BlockID.Void), true);
            }

            foreach (var inst in attacker.Level.ActiveInstances)
                if (inst is Vine vine && vine.TargetInstanceID == attacker.ID)
                    vine.Destroy();
        }
    }

    public class HealingAura : SpellEffectAcute
    {
        public HealingAura() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 2;
            float maxHeal = 8, minHeal = 5;
            int playerAmt = 0;

            // Heal entities
            foreach (Entity entity in attacker.Level.ActiveInstances.Where(i => i is Entity))
            {
                var dist = attacker.DistanceTo(entity).ChebyshevLength();
                if (dist <= range && attacker.CanAffect(entity, new Attack(attacker.Level.Server, Style.Ranged, 1, range, 1, 1)))
                {
                    var amt = entity.Heal((int)Util.LerpTo(minHeal, maxHeal, 1f - (dist / range)));
                    if (entity is Player player)
                        playerAmt = amt;
                }
            }

            var playerMsg = playerAmt > 0 ? " You have been healed for <lime>" + playerAmt + "<green> hp." : "";
            attacker.Level.Server.User.ShowMessage(
                "<green>" + specs.Item1 + " " + GetVerb(attacker, "have", "has") +
                " partially healed all creatures surrounding " + (attacker is Player ? "you" : "it") + "!" + playerMsg);
        }
    }

    public class Detonate : SpellEffectVector
    {
        public Detonate() { }

        public override void Apply(Entity attacker, Vector target)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 2;
            float maxDmg = 10, minDmg = 5;

            var order = attacker.Level.ActiveInstances.Where(i => i is Entity && i.DistanceTo(target).ChebyshevLength() <= range).ToList(); // && entity.CanSee(target)
            order.Sort((i, j) => j.DistanceTo(target).ChebyshevLength() - i.DistanceTo(target).ChebyshevLength());

            // Dmg entities
            foreach (var inst in order)
            {
                if (!(inst is Entity entity) || !entity.IsAttackable()) continue;

                // Roll vs res
                if (!GetSpc(attacker).RollAgainst(entity.GetRes().Round()))
                {
                    entity.AddHit(new HitString(entity, "Resist"));
                    continue;
                }

                var offset = entity.DistanceTo(target);
                var dist = offset.ChebyshevLength();
                if (dist <= range) // && entity.CanSee(target)
                {
                    var amt = entity.GetHit(Style.Other, (int)Util.LerpTo(minDmg, maxDmg, 1f - (dist / range)));
                    
                    var angle = - offset.ToFloat();
                    if (angle != Vector2.Zero) angle.Normalize();
                    else angle = Dir.Random.ToFloat();

                    var push = (angle * range).ToInt();
                    entity.Move(push);

                    entity.AddStatusEffect(new StatusEffects.Stunned(1, 3));

                    if (entity is Player player)
                        player.User.ShowMessage("You have taken <red>" + amt + "<r> dmg.");
                }
            }
        }
    }

    //public class Lightning : SpellEffectVector
    //{
    //    public Lightning() { }

    //    public override void Apply(Entity attacker, Vector target)
    //    {
    //        var specs = GetMsgAdresses(attacker, attacker);
    //        int range = 6;
    //        int dmg = 6;

    //        Dir dir = attacker.GetTiles().Contains(target) ? Dir.Random : (target - attacker.Position).ToDir();

    //        var centerPos = attacker.Position;
    //        List<Vector> nodes = new List<Vector>(range);
    //        for (int i = 1; i <= range; i++)
    //        {
    //            var pos = centerPos + new Vector(dir.X * i, dir.Y * i);
    //            if (!attacker.Level.GetTile(pos).IsAimable())
    //                break;

    //            // Don't go through closed doors:
    //            if (attacker.Level.ActiveInstances.Any(b => b.BlocksLight() && b.GetTiles().Contains(pos)))
    //                break;

    //            nodes.Add(pos);
    //        }

    //        attacker.Level.Server.User.ShowMessage("<aqua>" + GetMsgAdresses(attacker, attacker).Item1 + " " + GetVerb(attacker, "send") + " out a beam of lightning!");

    //        if (nodes.Count <= 0)
    //            return;

    //        // Add projectile
    //        attacker.Level.AddInstance(new LightningProjectile(attacker, nodes.First(), nodes.Last()));

    //        var targets = attacker.Level.ActiveInstances.Where(i => i.GetTiles().Intersect(nodes).Count() > 0).ToList();
    //        targets.Remove(attacker);
            
    //        foreach (var inst in targets)
    //        {
    //            if (!(inst is Entity entity) || !entity.IsAttackable()) continue;
                
    //            // Roll vs res
    //            if (!GetSpc(attacker).RollAgainst(entity.GetRes()))
    //            {
    //                entity.AddHit(new HitString(entity, "Resist"));
    //                continue;
    //            }

    //            var affect = new Attack(attacker.Level.Server, Style.Magic, 1, range, 20, dmg);
    //            entity.GetAffected(attacker, affect);
    //        }
    //    }
    //}

    public class Lightning : SpellEffectVector
    {
        public Lightning() { }

        public override void Apply(Entity attacker, Vector target)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 6;
            int dmg = 6;

            // Cast a ray of nodes, until max range or tile blocked by something.
            var nodes = Util.CastRay(attacker.GetTiles(), new Vector[] { target }, false, true);
            nodes = nodes.TakeWhile((n, i) =>
                    attacker.Level.GetTile(n).IsAimable() &&
                    !attacker.Level.ActiveInstances.Any(b => b.BlocksLight() && b.GetTiles().Contains(n)) &&
                    i < range
                );

            attacker.Level.Server.User.ShowMessage("<aqua>" + GetMsgAdresses(attacker, attacker).Item1 + " " + GetVerb(attacker, "send") + " out a beam of lightning!");

            if (nodes.Count() <= 0)
                return;

            // Add projectile
            attacker.Level.AddInstance(new LightningProjectile(attacker, nodes.ToArray()));

            var targets = attacker.Level.ActiveInstances.Where(i => i.GetTiles().Intersect(nodes).Count() > 0).ToList();
            targets.Remove(attacker);

            foreach (var inst in targets)
            {
                if (!(inst is Entity entity) || !entity.IsAttackable()) continue;

                // Roll vs res
                if (!GetSpc(attacker).RollAgainst(entity.GetRes().Round()))
                {
                    entity.AddHit(new HitString(entity, "Resist"));
                    continue;
                }

                var affect = new Attack(attacker.Level.Server, Style.Magic, 1, range, 20, dmg);
                entity.GetAffected(attacker, affect);
            }
        }
    }

    public class Dash : SpellEffectVector
    {
        public Dash() { }

        public override void Apply(Entity attacker, Vector target)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 4;

            //Vector dir = attacker.GetTiles().Contains(target) ? Vector.Zero : (target - attacker.Position).Sign();
            //var dist = attacker.DistanceTo(target).ChebyshevLength().Boundarize(-range, range);
            //var move = dir * dist;

            // Cast a ray of nodes, until max range or tile blocked by something.

            //var path = new Path(attacker, target, true);
            //var nodes = path.PathList.Select(p => p.ToVector()).ToList();

            //var movements = new List<Vector>();
            //var prev = attacker.Position;
            //var dist = 0;
            //while (nodes.Count() > 0 && dist < range)
            //{
            //    var node = nodes.First() - prev;
            //    nodes.RemoveAt(0);
            //    dist += node.ChebyshevLength();
            //    movements.Add(node);
            //}

            if (!attacker.CanMove())
            {
                attacker.Level.Server.User.ShowMessage("<rose>" + specs.Item1 + " " + GetVerb(attacker, "try", "tries") + " to dash, but " + GetVerb(attacker, "you", "it") + " cannot move!");
                return;
            }

            var nodes = Util.CastRay(new Vector[] { attacker.Position }, new Vector[] { target }, false, true);
            nodes = nodes.TakeWhile((n, i) =>
                    attacker.Level.GetTile(n).IsAimable() &&
                    !attacker.Level.ActiveInstances.Any(b => b.BlocksLight() && b.GetTiles().Contains(n)) &&
                    i <= range
                );


            var movements = new List<Vector>();
            var prev = attacker.Position;
            foreach (var node in nodes)
            {
                movements.Add(node - prev);
                prev = node;
            }
            attacker.Move(movements, false);

            attacker.Level.Server.User.ShowMessage("<yellow>" + specs.Item1 + " " + GetVerb(attacker, "dash", "dashes") + " away!");
        }
    }

    public class Teleport : SpellEffectVector
    {
        public Teleport() { }

        public override void Apply(Entity attacker, Vector target)
        {
            var specs = GetMsgAdressesHave(attacker, attacker);

            if (target == attacker.Position)
            {
                attacker.Level.Server.User.ShowMessage("<aqua>" + specs.Item1 + " teleported to where it already was!");
                return;
            }

            if (!attacker.CanSee(target))
            {
                if (attacker is Player) attacker.Level.Server.User.ShowMessage("<rose>You must be able to see where you want to teleport!");
                return;
            }

            if (!attacker.CanExist(target.X, target.Y))
            {
                if (attacker is Player) attacker.Level.Server.User.ShowMessage("<rose>You cannot teleport there!");
                return;
            }

            attacker.Position = target;
            attacker.Level.Server.User.ShowMessage("<aqua>" + specs.Item1 + " teleported away!");
        }
    }
    public class WindBlastAbstract : SpellEffect
    {
        protected int Strength;
        public WindBlastAbstract(int strength) { Strength = strength; }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            if (!(target is Entity entity))
            {
                attacker.Level.Server.User.ShowMessage(((target is Player) ? "you" : target.ToStringAdress()).CapFirst() + " remained unaffected by the wind blast.");
                return;
            }

            Vector dist = attacker.DistanceTo(target);
            Vector2 angle = dist.ToFloat();
            if (angle != Vector2.Zero) angle.Normalize();
            Vector2 push = new Vector(Strength).ToFloat() * angle;

            // Ceil push vector values (floors for negative values):
            Vector pushVector = new Vector(Math.Sign(push.X) * (int)Math.Ceiling(Math.Abs(push.X)), Math.Sign(push.Y) * (int)Math.Ceiling(Math.Abs(push.Y)));
            entity.Move(pushVector);

            entity.AddStatusEffect(new StatusEffects.Stunned(1, 3), true);

            if (entity is DustCloud dc)
                dc.Angle = pushVector.Sign();

            var specs = GetMsgAdresses(attacker, target);
            attacker.Level.Server.User.ShowMessage("<yellow>" + specs.Item1 + " " + GetVerb(attacker, "blow") + " " + specs.Item2 + " away.");
        }
    }

    public class WindBlast : WindBlastAbstract
    {
        public WindBlast() : base(4) { }
    }

    public class SmallWindBlast : WindBlastAbstract
    {
        public SmallWindBlast() : base(2) { }
    }

    public class Whirlwind : SpellEffectAcute
    {
        public Whirlwind() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 3;

            var order = attacker.Level.ActiveInstances.Where(i => i is Entity && i.DistanceTo(attacker).ChebyshevLength() <= range && attacker.CanSee(i)).ToList();
            
            order.Sort((i, j) => j.DistanceTo(attacker).ChebyshevLength() - i.DistanceTo(attacker).ChebyshevLength());

            attacker.Level.Server.User.ShowMessage("<medblue>" + GetMsgAdresses(attacker, attacker).Item1 + " " + GetVerb(attacker, "send") + " out a whirlwind!");

            // Blow entities
            foreach (var inst in order)
            {
                if (!(inst is Entity entity) || inst == attacker) continue;

                

                // Roll vs res
                if (!GetSpc(attacker).RollAgainst(entity.GetRes().Round()))
                {
                    entity.AddHit(new HitString(entity, "Resist"));
                    continue;
                }

                var offset = entity.DistanceTo(attacker);
                var dist = offset.ChebyshevLength();
                if (dist <= range) // && entity.CanSee(target)
                {
                    var angle = -offset.ToFloat();
                    if (angle != Vector2.Zero) angle.Normalize();
                    else angle = Dir.Random.ToFloat();

                    var push = (angle * range).ToInt();
                    entity.Move(push);
                    entity.AddStatusEffect(new StatusEffects.Stunned(1, 3));

                    if (entity is DustCloud dc)
                        dc.Angle = push.Sign();

                    if (entity is Player player)
                    {
                        player.User.ShowMessage("<yellow>You were blown away by " + specs.Item1 + ".");
                    }
                }
            }
        }
    }

    public class Entangle : SpellEffect
    {
        public Entangle() { }

        public override void Apply(Entity attacker, Instance target, Affect affect)
        {
            if (!(target is Entity entity))
                return;


            var effect = new StatusEffects.Frozen(1, 2, "entangled");
            var success = entity.AddStatusEffect(effect);

            if (success)
            {
                var specs = GetMsgAdressesHave(attacker, target);

                // Cast a ray to the target
                var vine = new Vine(attacker, target);
                attacker.Level.CreateInstance(vine);
                attacker.Children.Add(vine.CreateRef());

                attacker.Level.Server.User.ShowMessage("<red>" + specs.Item1 + " entangled " + specs.Item2 + " with a vine!");
            }
        }
    }

    public class Cure : SpellEffectAcute
    {
        public Cure() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            attacker.Level.Server.User.ShowMessage("<green>" + specs.Item1 + " " + GetVerb(attacker, "cure") + " " + specs.Item2 + ".");

            var removeIDs = new int[] { StatusEffectID.Poison, StatusEffectID.Blind, StatusEffectID.Bleeding, StatusEffectID.Nausea };

            attacker.StatusEffects.FilterOut(s => !removeIDs.Contains(s.GetID()));
        }
    }

    public class Charge : SpellEffectAcute
    {
        public Charge() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            attacker.Level.Server.User.ShowMessage("<vred>" + specs.Item1 + " " + GetVerb(attacker, "cast") + " charge!");

            var effect = new StatusEffects.Invulnerable(1, 5);
            attacker.AddStatusEffect(effect, true);
        }
    }

    public class Scream : SpellEffectAcute
    {
        public Scream() { }

        public override void Apply(Entity attacker)
        {
            var specs = GetMsgAdresses(attacker, attacker);
            int range = 3;

            var order = attacker.Level.ActiveInstances.Where(i => i is Entity && i.DistanceTo(attacker).ChebyshevLength() <= range && attacker.CanSee(i)).ToList();

            attacker.Level.Server.User.ShowMessage("<red>" + GetMsgAdresses(attacker, attacker).Item1 + " " + GetVerb(attacker, "scream") + " very loudly!");

            // Affect entities
            foreach (var inst in order)
            {
                if (!(inst is Entity entity) || inst == attacker) continue;

                var dist = entity.DistanceTo(attacker).ChebyshevLength();
                if (dist <= range) // && entity.CanSee(target)
                {
                    entity.AddStatusEffect(new StatusEffects.Weak(3, 11));
                    entity.GetAffected(attacker, new DirectDamage(attacker.Level.Server, Style.Other, 8, true));

                    if (entity is Player player)
                    {
                        player.User.ShowMessage("<red>You have been severely weakened by the scream.");
                    }
                }
            }
        }
    }
}
