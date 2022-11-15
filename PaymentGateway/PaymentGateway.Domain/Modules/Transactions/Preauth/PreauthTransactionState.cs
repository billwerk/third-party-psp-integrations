using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Preauth;

public class PreauthTransactionState : TransactionStateBase
{
    public NonNegativeAmount AuthorizedAmount { get; init; }

    public PreauthTransactionState(NonNegativeAmount authorizedAmount, PaymentTransactionStatus status, DateTime receivedAt)
        : base(status, receivedAt) => AuthorizedAmount = authorizedAmount;

    public static PreauthTransactionState CreateInitialState() 
        => new(new NonNegativeAmount(0), PaymentTransactionStatus.Initiated, DateTime.UtcNow);

    protected override bool EqualsInDetails(TransactionStateBase other)
    {
        var preauthTransactionState = (PreauthTransactionState)other;
        return AuthorizedAmount.Equals(preauthTransactionState.AuthorizedAmount);
    }
}
