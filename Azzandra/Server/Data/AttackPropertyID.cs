using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class AttackPropertyID
    {
        public const int
            Fire = 0,
            Frost = 1,
            Healing = 2,
            Poison = 3,
            Enchanted = 4,
            Entangle = 5,
            Knockback = 6,
            Blind = 7,
            Slow = 8;

        private static readonly Dictionary<int, Type> AttackPropertyIDs = new Dictionary<int, Type>()
        {
            { Fire, typeof(AttackProperties.Fire) },
            { Frost, typeof(AttackProperties.Frost) },
            { Healing, typeof(AttackProperties.Healing) },
            { Poison, typeof(AttackProperties.Poison) },
            { Enchanted, typeof(AttackProperties.Enchanted) },
            { Entangle, typeof(AttackProperties.Entangle) },
            { Knockback, typeof(AttackProperties.Knockback) },
            { Blind, typeof(AttackProperties.Blind) },
            { Slow, typeof(AttackProperties.Slow) },
            //{ Smite, "smite" },
        };

        public static int GetID(this AttackProperty prop)
        {
            if (prop == null) return -1;
            return AttackPropertyIDs.FirstOrDefault(x => x.Value == prop.GetType()).Key;
        }

        public static Type GetType(int id)
        {
            if (AttackPropertyIDs.TryGetValue(id, out var t))
                return t;
            else
                return null;
        }

        /// <summary>
        /// Returns the 'camel_case' string representation of the given attack property type.
        /// Used for referring the property-type data objects.
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetTypeID(this AttackProperty prop)
        {
            // Get the type name -> replace capitals with '_' + lowercase character equivalent.
            var typeName = prop.GetType().Name;
            return typeName.ToUnderscore();
        }

        /// <summary>
        /// Returns the actual 'Type' of the instance class corresponding to the string in 'camel_case'.
        /// Used for referring to the instance-type data objects.
        /// </summary>
        /// <param name="id">The 'camel_case' string representation.</param>
        /// <returns></returns>
        public static Type GetType(string id)
        {
            id = id.ToCamelCase();

            // Find corresponding instance type:
            var type = Type.GetType("Azzandra.AttackProperties." + id);
            if (typeof(AttackProperty).IsAssignableFrom(type))
            {
                return type;
            }

            return null;
        }
    }
}
