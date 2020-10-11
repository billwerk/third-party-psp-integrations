using System;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Serializer
{
    public class ObjectIdConverter : JsonConverter<ObjectId>
    {
        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override ObjectId ReadJson(
            JsonReader reader,
            Type objectType,
            ObjectId existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = JToken.ReadFrom(reader);
            return ObjectId.Parse(value.ToString());
        }
    }
}