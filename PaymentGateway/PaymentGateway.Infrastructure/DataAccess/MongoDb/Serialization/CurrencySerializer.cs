using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class CurrencySerializer : IBsonSerializer<Currency>
{
    public Type ValueType => typeof(Currency);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object @object) => Serialize(context, args, (Currency)@object);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Currency value) => context.Writer.WriteString(value.Value);

    Currency IBsonSerializer<Currency>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => (Currency)Deserialize(context, args);

    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => new Currency(context.Reader.ReadString());
}
