using Modeller.Models;
using Newtonsoft.Json;
using System;

namespace Modeller.JsonConverters
{
    public class NameConverter : JsonConverter<Name>
    {
        public override Name ReadJson(JsonReader reader, Type objectType, Name existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            return new Name(s);
        }

        public override void WriteJson(JsonWriter writer, Name value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Singular.Value + string.Empty);
        }
    }
}
