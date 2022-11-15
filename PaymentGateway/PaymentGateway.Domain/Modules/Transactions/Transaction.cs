using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.Abstractions.DataAccess.DatabaseObjects;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.Transactions;

public abstract class Transaction : DatabaseObjectBase
{
    protected Transaction(Currency currency,
        BillwerkTransactionId billwerkTransactionId, Uri webhookUrl, PaymentMethodInfo paymentMethodInfo, NotEmptyString pspSettingsId) : base()
    {
        Currency = currency;
        BillwerkTransactionId = billwerkTransactionId;
        WebhookUrl = webhookUrl;
        PaymentMethodInfo = paymentMethodInfo;
        // ReSharper disable once VirtualMemberCallInConstructor -> this is abstract method which will be anyway
        // overridden, so this rather safe.
        States = new TransactionStateCollection<TransactionStateBase>(GetInitialTransactionState());
        PspSettingsId = pspSettingsId;
    }

    public Currency Currency { get; init; }

    public BillwerkTransactionId BillwerkTransactionId { get; init; }

    public Uri WebhookUrl { get; init; }

    public PaymentMethodInfo PaymentMethodInfo { get; init; }

    public NotEmptyString? PspTransactionId { get; set; }

    public TransactionStateCollection<TransactionStateBase> States { get; private set; }

    public NotEmptyString PspSettingsId { get; init; }

    public IDictionary<string, string>? PspTransactionData { get; set; }

    protected abstract TransactionStateBase GetInitialTransactionState();
}
