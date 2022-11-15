using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class PositiveAmountSerializer : IBsonSerializer
{
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) 
        => (PositiveAmount)(decimal)context.Reader.ReadDecimal128();


    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is PositiveAmount val)
            context.Writer.WriteDecimal128((decimal)val);
    }

    public Type ValueType => typeof(PositiveAmount);
}
