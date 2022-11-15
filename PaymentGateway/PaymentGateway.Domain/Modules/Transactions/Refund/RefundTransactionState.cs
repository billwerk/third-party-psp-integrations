using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;

namespace PaymentGateway.Domain.Modules.Transactions.Refund;

public class RefundTransactionState : TransactionStateBase
{
    public RefundTransactionState(PaymentTransactionStatus status, DateTime receivedAt) : base(status, receivedAt)
    {
    }

    public static RefundTransactionState GetInitialState() =>
        new (PaymentTransactionStatus.Initiated, DateTime.UtcNow);

    protected override bool EqualsInDetails(TransactionStateBase other) => true;
}
