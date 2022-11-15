using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Payment;

public class PaymentTransaction : Transaction
{
    public DateTime? DueDate { get; set; }
    
    public AgreementId AgreementId { get; init; }

    public PositiveAmount RequestedAmount { get; init; }

    public NotEmptyString ReferenceText { get; init; }

    public PaymentTransaction(
        PositiveAmount requestedAmount,
        Currency currency,
        BillwerkTransactionId billwerkTransactionId,
        Uri webhookUrl,
        PaymentMethodInfo paymentMethodInfo,
        AgreementId agreementId,
        NotEmptyString pspSettingsId,
        NotEmptyString referenceText) 
        : base(currency, billwerkTransactionId, webhookUrl, paymentMethodInfo, pspSettingsId)
    {
        AgreementId = agreementId;
        RequestedAmount = requestedAmount;
        ReferenceText = referenceText;
    }

    protected override TransactionStateBase GetInitialTransactionState() =>
        PaymentTransactionState.CreateInitialState();
}
