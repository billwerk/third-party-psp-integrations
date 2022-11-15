// Copyright (c) billwerk GmbH. All rights reserved

using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class NotEmptyStringSerializer : IBsonSerializer<NotEmptyString>
{
    public Type ValueType => typeof(NotEmptyString);

    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        new NotEmptyString(context.Reader.ReadString());

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, NotEmptyString value) =>
        context.Writer.WriteString(value.Value);

    NotEmptyString IBsonSerializer<NotEmptyString>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => (NotEmptyString)Deserialize(context, args);
    
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object @object) =>
        Serialize(context, args, (NotEmptyString)@object);
}
