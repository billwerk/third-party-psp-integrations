// Copyright (c) billwerk GmbH. All rights reserved

using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Infrastructure.JsonSerialization;

public class JsonDateTimeOffsetSerializer : JsonConverter<DateTimeOffset>
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffffffK";

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null
            ? default
            : reader.GetDateTimeOffset();

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset dateTimeValue,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(dateTimeValue.DateTime.ToString(Format));
}


