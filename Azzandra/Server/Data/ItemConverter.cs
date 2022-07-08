using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ItemConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Item);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //instantiate item based on type
            var itemData = JObject.Load(reader);
            var itemType = itemData.TryGetValue("type", out var type) ? type.Value<string>() : null;
            var instance = InstantiateItemByType(itemType);

            //populate item instance
            serializer.Populate(itemData.CreateReader(), instance);

            return instance;
        }


        //instantiate ItemType object based on string
        private Item InstantiateItemByType(string typeID)
        {
            // Retrieve the corresponding type, otherwise set it to generic "item".
            var type = Type.GetType("Azzandra.Items." + typeID.ToCamelCase());
            if (type == null || !typeof(Item).IsAssignableFrom(type))
                type = typeof(Item);

            return (Item)Activator.CreateInstance(type);
        }
    }
}
