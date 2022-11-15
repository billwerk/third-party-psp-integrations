// Copyright (c) billwerk GmbH. All rights reserved

using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.JsonSerialization;

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffffffZ";

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options) => writer.WriteStringValue(date.ToString(Format));

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType == JsonTokenType.Null
        ? default
        : reader.GetDateTime();
}
