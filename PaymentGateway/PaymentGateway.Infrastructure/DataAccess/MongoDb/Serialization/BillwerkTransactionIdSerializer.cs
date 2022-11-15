using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class BillwerkTransactionIdSerializer : IBsonSerializer
{
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        new BillwerkTransactionId(context.Reader.ReadString());

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object @object)
    {
        if (@object is BillwerkTransactionId billwerkTransactionId)
            context.Writer.WriteString(billwerkTransactionId.Value);
    }

    public Type ValueType => typeof(BillwerkTransactionId);
}
