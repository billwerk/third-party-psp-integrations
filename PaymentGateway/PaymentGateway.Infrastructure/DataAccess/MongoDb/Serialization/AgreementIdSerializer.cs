using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class AgreementIdSerializer : IBsonSerializer<AgreementId>
{
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) =>
        new AgreementId(context.Reader.ReadString());

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AgreementId value) =>
        context.Writer.WriteString(value.Value);

    AgreementId IBsonSerializer<AgreementId>.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        => (AgreementId)Deserialize(context, args);
    
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object @object) =>
        Serialize(context, args, (AgreementId)@object);

    public Type ValueType => typeof(AgreementId);
}
