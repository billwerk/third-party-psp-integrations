using System.Collections.Generic;
using Billwerk.Payment.SDK.Interfaces;
using Business.Serializer;
using Core.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Services
{
    public class JsonConvertService:IJsonConvertService
    {
        
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            FloatParseHandling = FloatParseHandling.Decimal,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            StringEscapeHandling = StringEscapeHandling.Default,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            DateParseHandling = DateParseHandling.None,
            Converters = new List<JsonConverter>
            {
                new StringConverter(),
                new DateTimeConverter(),
                new StringEnumConverter(),
                new DecimalJsonConverter(),
                new ObjectIdConverter(),
                new CultureInfoConverter(),
                new PaymentBearerJsonConverter(),
                new PspBearerJsonConverter()
            }
        };
        
        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _jsonSerializerSettings);
        }

        public string SerializeObject(object? value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }
    }
}