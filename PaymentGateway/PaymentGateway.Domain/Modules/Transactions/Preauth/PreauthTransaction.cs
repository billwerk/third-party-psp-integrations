using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions.Preauth;

public class PreauthTransaction : Transaction
{
    public Agreement.Agreement Agreement { get; init; }

    public bool IsInitial { get; init; }

    public NonNegativeAmount RequestedAmount { get; init; }

    public PreauthTransaction(
        Agreement.Agreement agreement,
        NonNegativeAmount requestedAmount,
        Currency currency,
        BillwerkTransactionId billwerkTransactionId,
        PaymentMethodInfo paymentMethodInfo,
        Uri webhookUrl,
        NotEmptyString pspSettingsId,
        bool isInitial) : base(currency, billwerkTransactionId, webhookUrl, paymentMethodInfo, pspSettingsId)
    {
        Agreement = agreement;
        IsInitial = isInitial;
        RequestedAmount = requestedAmount;
    }

    protected override PreauthTransactionState GetInitialTransactionState() =>
        PreauthTransactionState.CreateInitialState();

    /// <summary>
    /// PspTransactionId can be null in case of preauth without payment: Trial or PaymentChange
    /// </summary>
    /// <returns></returns>
    public NotEmptyString? GetPspTransactionIdOrNull() => PspTransactionId;
}
