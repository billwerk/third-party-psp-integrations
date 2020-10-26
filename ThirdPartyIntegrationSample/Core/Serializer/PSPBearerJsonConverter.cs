using System;
using Business.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistence.Models;

namespace Business.Serializer
{
    public class PspBearerJsonConverter : JsonConverter<PspBearer>
    {
        public override void WriteJson(JsonWriter writer, PspBearer value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override PspBearer ReadJson(JsonReader reader, Type objectType, PspBearer existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
            //TODO think about how to implement this correctly
            /*
            // Load JObject from stream
            var jObject = JObject.Load(reader);
            PspBearer obj = new PayOnePspBearer();

            serializer.Populate(jObject.CreateReader(), obj);

            return obj;
            */
        }

        public override bool CanWrite => false;
    }
}