using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Core.Serializer
{
    public class DecimalJsonConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (value)
            {
                case decimal decimalValue when Math.Abs(decimalValue % 1) == 0:
                    writer.WriteRawValue(decimalValue.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}