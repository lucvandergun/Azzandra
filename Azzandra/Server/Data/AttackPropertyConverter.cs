using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class AttackPropertyConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Instantiate property based on type
            JObject propData = JObject.Load(reader);
            string propID = propData.TryGetValue("id", out var type) ? type.Value<string>() : null;
            int propLevel = propData.TryGetValue("level", out var level) ? level.Value<int>() : 1;
            var propType = AttackPropertyID.GetType(propID);

            if (propType != null)
            {
                return (AttackProperty)Activator.CreateInstance(propType, propLevel);
            }

            return null;
        }


        //instantiate ItemType object based on string
        private AttackProperty InstantiateItemByType(string typeID)
        {
            // Retrieve the corresponding type, otherwise set it to generic "item".
            var type = Type.GetType("Azzandra.AttackProperty." + typeID.ToCamelCase());
            if (type == null || !typeof(AttackProperty).IsAssignableFrom(type))
                type = typeof(AttackProperty);

            return (AttackProperty)Activator.CreateInstance(type);
        }
    }
}
