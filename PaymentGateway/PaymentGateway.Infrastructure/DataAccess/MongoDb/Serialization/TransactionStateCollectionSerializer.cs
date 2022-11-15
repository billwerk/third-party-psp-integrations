using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public class TransactionStateCollectionSerializer : IBsonSerializer
{
    public Type ValueType => typeof(TransactionStateCollection<TransactionStateBase>);
    
    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadStartArray();

        var result = new List<TransactionStateBase>();

        while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            context.Reader.ReadStartDocument();
            var typeDiscriminatorName = context.Reader.ReadName();
            if (typeDiscriminatorName != "_t")
                throw new InvalidDataException("First field must contain bson type discriminator");

            var type = context.Reader.ReadString(); 
                
            TransactionStateBase deserializedTransactionState = type switch
            {
                nameof(PreauthTransactionState) => BsonSerializer.Deserialize<PreauthTransactionState>(context.Reader),
                nameof(TransactionErrorState) => BsonSerializer.Deserialize<TransactionErrorState>(context.Reader),
                nameof(PaymentTransactionState) => BsonSerializer.Deserialize<PaymentTransactionState>(context.Reader),
                nameof(RefundTransactionState) => BsonSerializer.Deserialize<RefundTransactionState>(context.Reader),
                _ => throw new NotImplementedException(),
            };
            
            result.Add(deserializedTransactionState);
            context.Reader.ReadEndDocument();
        }

        context.Reader.ReadEndArray();

        if (!result.Any())
            throw new InvalidDataException("States collection is empty");

        var statesCollection = new TransactionStateCollection<TransactionStateBase>(result[0]);
        foreach (var state in result.Skip(1))
            statesCollection.Add(state);

        return statesCollection;
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        context.Writer.WriteStartArray();
        
        foreach(var state in (TransactionStateCollection<TransactionStateBase>)value)
        {
            Action serializeAction = state switch
            {
                PreauthTransactionState preauthTransactionState => () => BsonSerializer.Serialize(context.Writer, preauthTransactionState),
                TransactionErrorState transactionErrorState => () => BsonSerializer.Serialize(context.Writer, transactionErrorState),
                PaymentTransactionState paymentTransactionState => () => BsonSerializer.Serialize(context.Writer, paymentTransactionState),
                RefundTransactionState refundTransactionState => () => BsonSerializer.Serialize(context.Writer, refundTransactionState),
                _ => throw new NotImplementedException(),
            };
            
            context.Writer.WriteStartDocument();
            serializeAction();
            context.Writer.WriteEndDocument();
        }
        
        context.Writer.WriteEndArray();
    }
}
