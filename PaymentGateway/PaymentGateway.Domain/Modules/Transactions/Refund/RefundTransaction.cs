using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Refund;

public class RefundTransaction : Transaction
{
    public BillwerkTransactionId TargetPaymentTransactionId { get; init; }

    public PositiveAmount RequestedAmount { get; init; }

    protected override TransactionStateBase GetInitialTransactionState() 
        => RefundTransactionState.GetInitialState();

    public RefundTransaction(PositiveAmount requestedAmount,
        Currency currency,
        BillwerkTransactionId billwerkTransactionId,
        BillwerkTransactionId targetPaymentTransactionId,
        Uri webhookUrl, 
        PaymentMethodInfo paymentMethodInfo,
        NotEmptyString pspSettingsId) 
        : base(currency, billwerkTransactionId, webhookUrl, paymentMethodInfo, pspSettingsId)
    {
        TargetPaymentTransactionId = targetPaymentTransactionId;
        RequestedAmount = requestedAmount;
    }
}
