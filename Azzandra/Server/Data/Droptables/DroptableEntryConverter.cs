using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DroptableEntryConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DroptableEntry);
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Determine what kind of entry it is:
            var dataEntry = JObject.Load(reader);
            DroptableEntry entry;

            if (dataEntry.ContainsKey("table"))
                entry = new DroptableDrop();
            else if (dataEntry.ContainsKey("items"))
                entry = new MultipleDrop();
            else
                entry = new SingleDrop();

            serializer.Populate(dataEntry.CreateReader(), entry);

            return entry;
        }
    }
}
