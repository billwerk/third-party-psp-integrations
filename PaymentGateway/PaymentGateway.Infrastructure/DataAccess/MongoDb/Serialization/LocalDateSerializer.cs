// Copyright (c) billwerk GmbH. All rights reserved

using System.Globalization;
using MongoDB.Bson.Serialization;
using NodaTime;
using NodaTime.Text;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class LocalDateSerializer : IBsonSerializer<LocalDate>
{
    public Type ValueType => typeof(LocalDate);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, LocalDate value) =>
        context.Writer.WriteString(value.ToString(LocalDatePattern.Iso.PatternText, CultureInfo.InvariantCulture));

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) =>
        Serialize(context, args, (LocalDate)value);

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        Deserialize(context, args);

    public LocalDate Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        LocalDatePattern.Iso.Parse(context.Reader.ReadString()).Value;
}
