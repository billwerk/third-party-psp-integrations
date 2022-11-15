using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState;

public class TransactionErrorState : TransactionStateBase
{
    public string ErrorMessage { get; init; }

    public PaymentErrorCode ErrorCode { get; }

    public string? PspErrorCode { get; }

    public TransactionErrorState(DateTime receivedAt, string errorMessage, PaymentErrorCode errorCode, string? pspErrorCode)
        : base(PaymentTransactionStatus.Failed, receivedAt)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        PspErrorCode = pspErrorCode;
    }

    protected override bool EqualsInDetails(TransactionStateBase other)
    {
        var errorState = (TransactionErrorState)other;
        return ErrorMessage.Equals(errorState.ErrorMessage);
    }
}
