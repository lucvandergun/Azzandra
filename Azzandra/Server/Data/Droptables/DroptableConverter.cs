using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DroptableConverter : JsonConverter
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
            // Fill droptable.Table value:
            var table = JArray.Load(reader);
            var droptable = new Droptable() { Table = new DroptableEntry[table.Count] };

            for (int i = 0; i < table.Count; i++)
            {
                droptable.Table[i] = table[i].ToObject<DroptableEntry>();
            }

            return droptable;
        }
    }
}
