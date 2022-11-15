using Billwerk.Payment.SDK.Enums;
using MongoDB.Bson.Serialization.Attributes;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;

//In billwerk we used smth like ITransactionResponseDto as base for revisions (transaction state here)
//I don't see any reason, to copypaste inside revision immutable for transaction itself, data, like:
//Currency, RequestedAmount, PspTransactionId(?), BillwerkTransactionId
//This fields must not be changed during whole transaction lifecycle!
//Normally, we just receive new status (in ideal case).

[BsonDiscriminator(Required = true)]
[BsonKnownTypes(typeof(PreauthTransactionState), 
    typeof(PaymentTransactionState), 
    typeof(RefundTransactionState),
    typeof(TransactionErrorState))]
public abstract class TransactionStateBase
{
    public PaymentTransactionStatus Status { get; init; }
    public DateTime ReceivedAt { get; init; }

    private DateTime _lastSeenAt;
    public DateTime LastSeenAt
    {
        get => _lastSeenAt;
        set
        {
            if (value < _lastSeenAt)
                throw new ArgumentException($"Can't set last seen time to {value} which is less than {LastSeenAt}");

            _lastSeenAt = value;
        }
    }

    protected TransactionStateBase(PaymentTransactionStatus status, DateTime receivedAt)
    {
        Status = status;
        ReceivedAt = receivedAt;
        LastSeenAt = ReceivedAt;
    }
    
    public bool EqualsTo(TransactionStateBase other) =>
        GetType() == other.GetType() &&
        Status == other.Status &&
        EqualsInDetails(other);

    protected abstract bool EqualsInDetails(TransactionStateBase other);
}
