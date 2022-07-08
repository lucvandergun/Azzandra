using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class InstanceID
    {
        /// <summary>
        /// Returns the simple 'CamelCase' string representation of the instance's class type.
        /// Used for saving.
        /// </summary>
        /// <param name="inst">The instance to take the type from.</param>
        /// <returns></returns>
        public static string GetTypeID(this Instance inst)
        {
            return inst.GetType().Name;
        }

        /// <summary>
        /// Returns the actual 'Type' of the instance class corresponding to the string in 'CamelCase'.
        /// Used for saving.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Type GetType(string id)
        {
            var type = Type.GetType("Azzandra." + id);
            if (typeof(Instance).IsAssignableFrom(type))
            {
                return type;
            }

            return null;
        }

        /// <summary>
        /// Returns the 'camel_case' string representation of the instance's class type.
        /// Used for referring the instance-type data objects.
        /// </summary>
        /// <param name="inst"></param>
        /// <returns></returns>
        public static string GetTypeID2(this Instance inst)
        {
            // Get the type name -> replace capitals with '_' + lowercase character equivalent.
            var typeName = inst.GetType().Name;
            return typeName.ToUnderscore();
        }

        /// <summary>
        /// Returns the actual 'Type' of the instance class corresponding to the string in 'camel_case'.
        /// Used for referring to the instance-type data objects.
        /// </summary>
        /// <param name="id">The 'camel_case' string representation.</param>
        /// <returns></returns>
        public static Type GetType2(string id)
        {
            id = id.ToCamelCase();

            // Find corresponding instance type:
            var type = Type.GetType("Azzandra." + id);
            if (typeof(Instance).IsAssignableFrom(type))
            {
                return type;
            }

            return null;
        }


        //public static Instance CreateInstanceFromID(string id, byte[] bytes, ref int pos)
        //{
        //    switch (id)
        //    {
        //        case "wolf": return new Wolf(bytes, pos);
        //        case "icegiant": return new IceGiant(bytes, pos);
        //        case "iceelemental": return new IceElemental(bytes, pos);
        //    }

        //    return null;
        //}
    }
}
