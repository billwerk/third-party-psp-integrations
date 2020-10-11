using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Serializer
{
    public class DateTimeConverter : IsoDateTimeConverter
    {
        private static readonly CultureInfo DefaultCulture = new CultureInfo("de");
        private static readonly Regex TimeZoneOffsetInfoRegex = new Regex("([+-](?:2[0-3]|[0-1][0-9]):[0-5][0-9])");
        private const string UtcSuffix = "Z";
        private const string DateFormat = "yyyy-MM-dd";
        public DateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffK";
        }
        
        private static DateTimeKind GetDateTimeKind(string dateTimeStr)
        {
            var startOfTz = TimeZoneOffsetInfoRegex.Match(dateTimeStr);
            if (startOfTz.Index > 0)
            {
                return DateTimeKind.Local;
            }
            return dateTimeStr.Contains(UtcSuffix) ? DateTimeKind.Utc : DateTimeKind.Unspecified;
        }

        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var nullable = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
            var type = (nullable) ? Nullable.GetUnderlyingType(objectType) : objectType;

            if (reader.TokenType == JsonToken.Null || type == typeof(DateTimeOffset))
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            DateTime parsedDateTime;
            var dateText = reader.Value.ToString();
            var receivedDateTimeKind = GetDateTimeKind(dateText);
            if (dateText.Length == DateFormat.Length)
            {
                parsedDateTime = DateTime.ParseExact(dateText, DateFormat, DefaultCulture, DateTimeStyles.RoundtripKind);
            }
            else if (receivedDateTimeKind != DateTimeKind.Utc || dateText.Length<DateTimeFormat.Length)
            {
                parsedDateTime = DateTime.Parse(dateText, DefaultCulture, DateTimeStyles.RoundtripKind);
            }
            else
            {
                parsedDateTime = (DateTime)base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (parsedDateTime.Kind != DateTimeKind.Utc)
            {
                parsedDateTime = parsedDateTime.ToUniversalTime();
            }
            
            return parsedDateTime;
        }
    }
}