using System.Globalization;
using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class NonNegativeAmountSerializer : IBsonSerializer
{
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        (NonNegativeAmount)decimal.Parse(context.Reader.ReadDecimal128().ToString(), CultureInfo.InvariantCulture);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        if (value is NonNegativeAmount val)
            context.Writer.WriteDecimal128((decimal)val);
    }

    public Type ValueType => typeof(NonNegativeAmount);
}
