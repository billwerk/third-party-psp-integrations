// Copyright (c) billwerk GmbH. All rights reserved

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;
using NodaTime.Text;

namespace PaymentGateway.Infrastructure.JsonSerialization;

public class JsonLocalDateConverter : JsonConverter<LocalDate>
{
    public override LocalDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null ? default : LocalDatePattern.Iso.Parse(reader.GetString()!).Value;

    public override void Write(Utf8JsonWriter writer, LocalDate localDate, JsonSerializerOptions options) =>
        writer.WriteStringValue(localDate.ToString(LocalDatePattern.Iso.PatternText, CultureInfo.InvariantCulture));

    public override bool CanConvert(Type objectType) => objectType == typeof(LocalDate);
}
