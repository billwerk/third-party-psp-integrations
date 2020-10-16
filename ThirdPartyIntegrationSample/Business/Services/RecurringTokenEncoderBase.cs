using System.Collections.Generic;
using Business.Interfaces;
using Business.Serializer;
using Core.Serializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Business.Services
{
    public abstract class RecurringTokenEncoderBase<T> : IRecurringTokenEncoder<T>
    {
        private readonly ITetheredPaymentInformationEncoder _tetheredPaymentInformationEncoder;

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

        protected RecurringTokenEncoderBase(ITetheredPaymentInformationEncoder tetheredPaymentInformationEncoder)
        {
            _tetheredPaymentInformationEncoder = tetheredPaymentInformationEncoder;
        }

        public string Encrypt(T token)
        {
            return _tetheredPaymentInformationEncoder.Encrypt(JsonConvert.SerializeObject(token, _jsonSerializerSettings));
        }

        public T Decrypt(string base64String)
        {
            return JsonConvert.DeserializeObject<T>(_tetheredPaymentInformationEncoder.Decrypt(base64String), _jsonSerializerSettings);
        }
    }
}