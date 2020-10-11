using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Core.Serializer
{
    public class CultureInfoConverter : JsonConverter<CultureInfo>
    {
        public override void WriteJson(JsonWriter writer, CultureInfo value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Name);
        }

        public override CultureInfo ReadJson(
            JsonReader reader,
            Type objectType,
            CultureInfo existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;
    }
}