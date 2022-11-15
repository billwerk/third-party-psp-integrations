using Billwerk.Payment.SDK.Enums;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

public abstract class TransactionStateBaseBsonSerializer<TState> : IBsonSerializer<TState> where TState : TransactionStateBase
{
    public Type ValueType => typeof(TState);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value) =>
        Serialize(context, args, (TState)value);

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, TState value)
    {
        context.Writer.WriteName("_t");
        context.Writer.WriteString(value.GetType().Name);
        
        context.Writer.WriteName(nameof(value.Status));
        context.Writer.WriteInt32((int)value.Status);
        
        context.Writer.WriteName(nameof(value.ReceivedAt));
        DateTimeSerializer.UtcInstance.Serialize(context, args, value.ReceivedAt);
        
        context.Writer.WriteName(nameof(value.LastSeenAt));
        DateTimeSerializer.UtcInstance.Serialize(context, args, value.LastSeenAt);

        ContinueWrite(context, value);
    }

    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => Deserialize(context, args);

    public TState Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        context.Reader.ReadName();
        var status = context.Reader.ReadInt32();
        
        context.Reader.ReadName();
        var receivedAt =  DateTimeSerializer.UtcInstance.Deserialize(context, args);
        
        context.Reader.ReadName();
        var lastSeenAt = DateTimeSerializer.UtcInstance.Deserialize(context, args);

        var state = ContinueReadAndCreateObject(context, (PaymentTransactionStatus)status, receivedAt);
        state.LastSeenAt = lastSeenAt;

        return state;
    }

    protected abstract void ContinueWrite(BsonSerializationContext context, TState state);

    protected abstract TState ContinueReadAndCreateObject(BsonDeserializationContext context, PaymentTransactionStatus status, DateTime receivedAt);
}

public class PreauthTransactionStateSerializer : TransactionStateBaseBsonSerializer<PreauthTransactionState>
{
    protected override void ContinueWrite(BsonSerializationContext context, PreauthTransactionState state)
    {
        context.Writer.WriteName(nameof(state.AuthorizedAmount));
        context.Writer.WriteDecimal128(state.AuthorizedAmount.Value);
    }

    protected override PreauthTransactionState ContinueReadAndCreateObject(BsonDeserializationContext context, PaymentTransactionStatus status, DateTime receivedAt)
    {
        context.Reader.ReadName();
        var authorizedAmount = context.Reader.ReadDecimal128();
        var amount = Convert.ToDecimal(authorizedAmount);

        return new PreauthTransactionState(new NonNegativeAmount(amount), status, receivedAt);
    }
}

public class TransactionErrorStateSerializer : TransactionStateBaseBsonSerializer<TransactionErrorState>
{
    protected override void ContinueWrite(BsonSerializationContext context, TransactionErrorState state)
    {
        context.Writer.WriteName(nameof(state.ErrorMessage));
        context.Writer.WriteString(state.ErrorMessage);
        context.Writer.WriteName(nameof(state.ErrorCode));
        context.Writer.WriteInt32((int)state.ErrorCode);
        context.Writer.WriteName(nameof(state.PspErrorCode));
        if (state.PspErrorCode == null)
            context.Writer.WriteNull();
        else
            context.Writer.WriteString(state.PspErrorCode);
    }

    protected override TransactionErrorState ContinueReadAndCreateObject(BsonDeserializationContext context, PaymentTransactionStatus status, DateTime receivedAt)
    {
        context.Reader.ReadName();
        var errorMessage = context.Reader.ReadString();

        context.Reader.ReadName();
        var errorCode = context.Reader.ReadInt32();

        string? pspErrorCode = null;
        if (context.Reader.ReadBsonType() == BsonType.Null)
        {
            context.Reader.ReadNull();
        }
        else
        {
            pspErrorCode = context.Reader.ReadString();
        }

        return new TransactionErrorState(receivedAt, errorMessage, (PaymentErrorCode)errorCode, pspErrorCode);
    }
}

public class PaymentTransactionStateSerializer : TransactionStateBaseBsonSerializer<PaymentTransactionState>
{
    protected override void ContinueWrite(BsonSerializationContext context, PaymentTransactionState state)
    {
        context.Writer.WriteName(nameof(state.Payments));
        BsonSerializer.Serialize(context.Writer, state.Payments);

        context.Writer.WriteName(nameof(state.Refunds));
        BsonSerializer.Serialize(context.Writer, state.Refunds);

        context.Writer.WriteName(nameof(state.Chargebacks));
        BsonSerializer.Serialize(context.Writer, state.Chargebacks);
    }

    protected override PaymentTransactionState ContinueReadAndCreateObject(BsonDeserializationContext context, PaymentTransactionStatus status, DateTime receivedAt)
    {
        context.Reader.ReadName();
        var paymentItems = BsonSerializer.Deserialize<List<PaymentItem>>(context.Reader);

        context.Reader.ReadName();
        var refundItems = BsonSerializer.Deserialize<List<RefundItem>>(context.Reader);

        context.Reader.ReadName();
        var chargebackItems = BsonSerializer.Deserialize<List<ChargebackItem>>(context.Reader);

        return new PaymentTransactionState(status, receivedAt, paymentItems, refundItems, chargebackItems);
    }
}

public class RefundTransactionStateSerializer : TransactionStateBaseBsonSerializer<RefundTransactionState>
{
    protected override void ContinueWrite(BsonSerializationContext context, RefundTransactionState state){ }
    
    protected override RefundTransactionState ContinueReadAndCreateObject(BsonDeserializationContext context, PaymentTransactionStatus status, DateTime receivedAt) => new(status, receivedAt);
}
