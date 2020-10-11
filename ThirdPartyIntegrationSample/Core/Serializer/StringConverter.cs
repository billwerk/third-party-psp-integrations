using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Serializer
{
    public class StringConverter : JsonConverter<string>
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            //try to handle "PropName":{} as null
            if (token.Type == JTokenType.Object)
            {
                return null;
            }

            return token.Value<string>();
        }
    }
}