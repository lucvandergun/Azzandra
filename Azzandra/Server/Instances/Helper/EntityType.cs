using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public enum EntityType
    {
        None,
        Humanoid,

        Fire,
        Ice,
        Acid,
        Shadow,
        Chaos,
        Metal,
        Rock,
        Plant,

        Undead,
        Skeleton,
        Spirit,
        Demon,
        Beast,
        Spider,

        Vampire,
        Troll,
        Goblin,
        NonPhysical
    }

    public static class EntityTypeHelper
    {
        private static readonly Dictionary<EntityType, EntityType[]> TypeInheritances = new Dictionary<EntityType, EntityType[]>()
        {
            { EntityType.Spirit, new EntityType[]{ EntityType.NonPhysical, EntityType.Undead } },
            { EntityType.Vampire, new EntityType[]{ EntityType.Humanoid } },
            { EntityType.Shadow, new EntityType[]{ EntityType.NonPhysical } },
        };

        private static readonly Dictionary<EntityType, int, string> TypeImmunities = new Dictionary<EntityType, int, string>
        {
            { EntityType.Fire, StatusEffectID.Burning, null },
            { EntityType.Ice, StatusEffectID.Frozen, "frozen" },
            { EntityType.Ice, StatusEffectID.Frostbite, null },
            { EntityType.NonPhysical, StatusEffectID.Burning, null },
            { EntityType.NonPhysical, StatusEffectID.Frozen, null },
            { EntityType.NonPhysical, StatusEffectID.Poison, null },
            { EntityType.NonPhysical, StatusEffectID.Bleeding, null },
            { EntityType.Undead, StatusEffectID.Poison, null },
            { EntityType.Demon, StatusEffectID.Burning, null },
        };

        private static readonly Dictionary<EntityType, int, string> TypeVulnerabilities = new Dictionary<EntityType, int, string>
        {
            { EntityType.Fire, StatusEffectID.Frozen, "frozen" },
            { EntityType.Fire, StatusEffectID.Frostbite, null },
            { EntityType.Ice, StatusEffectID.Burning, null },
        };


        /// <summary>
        /// This method returns whether supplied entity type is a form/child of supplied parent entity type.
        /// </summary>
        public static bool IsTypeOf(this EntityType type, EntityType parent)
        {
            if (type == parent)
                return true;

            foreach (var t in type.GetParentTypes())
            {
                if (t == parent)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the entity is either directly or by inheritance a specific entity type.
        /// </summary>
        /// <returns>Returns whether the entity is of a specific entity type.</returns>
        public static bool IsTypeOf(this Entity entity, EntityType type)
        {
            return entity.EntityType.IsTypeOf(type);
        }

        /// <summary>
        /// Gets all the entities' (inherited) entity types.
        /// </summary>
        public static List<EntityType> GetEntityTypes(this Entity entity)
        {
            var types = new List<EntityType>(2) { entity.EntityType };
            types.AddRange(GetParentTypes(entity.EntityType));

            return types;
        }

        /// <summary>
        /// Gets all the inherited entity types for that type.
        /// </summary>
        private static List<EntityType> GetParentTypes(this EntityType type)
        {
            var list = new List<EntityType>() { type };
            if (TypeInheritances.TryGetValue(type, out var parents))
                foreach (var parent in parents)
                    list.AddRange(parent.GetParentTypes());
            
            return list;
        }

        

        public static bool IsImmune(this Entity entity, int statusID, string name)
        {
            foreach (var type in GetEntityTypes(entity))
                if (IsTypeImmune(type, statusID, name))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether this specific entity type is immune to a status effect, does not check for inherited immunities.
        /// </summary>
        /// <returns>Returns status effect immunity.</returns>
        private static bool IsTypeImmune(this EntityType type, int statusID, string name)
        {
            if (TypeImmunities.TryGetValue(Tuple.Create(type, statusID), out string immuneType))
                return immuneType == null || immuneType == name;

            return false;
        }

        public static bool IsVulnerable(this Entity entity, int statusID, string name)
        {
            foreach (var type in GetEntityTypes(entity))
                if (IsTypeVulnerable(type, statusID, name))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks whether this specific entity type is immune to a status effect, does not check for inherited immunities.
        /// </summary>
        /// <returns>Returns status effect immunity.</returns>
        private static bool IsTypeVulnerable(this EntityType type, int statusID, string name)
        {
            if (TypeVulnerabilities.TryGetValue(Tuple.Create(type, statusID), out string vulnerableType))
                return vulnerableType == null || vulnerableType == name;

            return false;
        }



        /// <summary>
        /// This method modifies the referenced attack based on the target's entity types.
        /// </summary>
        /// <returns>Returns false if the attack has failed, i.e. damage roll should be discarded.</returns>
        public static bool ModifyByEntityTypes(this Affect affect, Entity target)
        {
            var types = GetEntityTypes(target);

            foreach (var type in types)
            {
                if (!ModifyByEntityType(target, affect, type))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This method modifies the referenced attack based on the target's entity type.
        /// </summary>
        /// <returns>Returns false if the attack has failed, i.e. damage roll should be discarded.</returns>
        private static bool ModifyByEntityType(this Entity target, Affect affect, EntityType type)
        {
            switch (type)
            {
                default: return true;
                case EntityType.Skeleton:
                    return Skeleton(affect, target);
                case EntityType.Undead:
                    return Undead(affect, target);
                case EntityType.NonPhysical:
                    return NonPhysical(affect, target);
                case EntityType.Fire:
                    return Fire(affect, target);
                case EntityType.Ice:
                    return Ice(affect, target);
                case EntityType.Shadow:
                    return Shadow(affect, target);
            }
        }

        private static bool Skeleton(Affect affect, Entity target)
        {
            if (affect is Attack attack && attack.Style == Style.Ranged)
            {
                affect.Fail("Your projectile goes straight trough " + target.ToStringAdress() + ".");
                affect.HitType = HitType.Evaded;
                return false;
            }

            return true;
        }

        private static bool Undead(Affect affect, Entity target)
        {
            return true;
        }

        private static bool Fire(Affect affect, Entity target)
        {
            if (affect is Attack attack && (affect.Properties?.Any(p => p.GetID() == AttackPropertyID.Fire) ?? true))
            {
                affect.Fail("<orange>" + target.ToStringAdress().CapFirst() + " seems to remain unaffected by your fire attack!");
                return false;
            }

            return true;
        }

        private static bool Ice(Affect affect, Entity target)
        {
            if (affect is Attack attack && (affect.Properties?.Any(p => p.GetID() == AttackPropertyID.Frost) ?? true))
            {
                affect.Fail("<ltblue>" + target.ToStringAdress().CapFirst() + " seems to remain unaffected by your frost attack!");
                return false;
            }

            return true;
        }

        private static bool Shadow(Affect affect, Entity target)
        {
            if (affect is Attack attack && (affect.Properties?.Any(p => p.GetID() == AttackPropertyID.Shadow) ?? true))
            {
                affect.Fail("<purple>" + target.ToStringAdress().CapFirst() + " seems to remain unaffected by your shadow attack!");
                return false;
            }

            return true;
        }

        private static bool NonPhysical(Affect affect, Entity target)
        {
            if (!(affect is Attack attack))
                return true;

            if (!attack.HasProperty(AttackPropertyID.Enchanted) && attack.Style != Style.Magic)
            {
                attack.Fail("<medblue>You attack " + target.ToStringAdress() + ", but your weapon goes right through it!");
                return false;
            }

            return true;
        }
    }
}
