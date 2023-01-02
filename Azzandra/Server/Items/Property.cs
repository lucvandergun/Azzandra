using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public abstract class Property
    {
        protected const int AttackID = 0, DefenseID = 1, FoodID = 2;

        public bool IsHidden { get; set; } = false;

        public virtual int GeneralTypeID => -1;
        
        public static Property Load(byte[] bytes)
        {
            var typeID = BitConverter.ToInt32(bytes, 0);
            int pos = 4;

            switch (typeID)
            {
                default:
                    return null;
                case AttackID:
                    return AttackProperty.Load(bytes, ref pos);
                case DefenseID:
                    return null;
                case FoodID:
                    return FoodEffect.Load(bytes, ref pos);
            }
        }

        public static byte[] Save(Property p)
        {
            var bytes = BitConverter.GetBytes(p.GeneralTypeID);
            return bytes.Concat(p.ToBytes()).ToArray();
        }

        public virtual byte[] ToBytes()
        {
            return new byte[0];
        }
    }
}
