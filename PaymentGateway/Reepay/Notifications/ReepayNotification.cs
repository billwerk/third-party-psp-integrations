// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace Reepay.Notifications;

internal record ReepayNotification(
    string Id,
    string Event_Type,
    string Timestamp,
    string Signature,
    string Invoice,
    string Transaction) : ITypedNotification
{
    /// <summary>
    /// Constant string which needed to fill PspTransactionId in a case, when Reepay send to us notifications which not connected with transactions
    /// (subscription / invoice dunning / customer events). No transaction will be fetched by notification handling mechanism ->
    /// notification will be ignored. Valid workaround since we use Subscription platform as clean PSP.
    /// </summary>
    private const string EmptyTransactionIdentifier = "<EmptyReepayId>";

    public TransactionNotification ToTransactionNotification(string rawData) => new(PaymentProvider.Reepay,
        new NotEmptyString(rawData),
        this,
        GetPspTransactionId());

    public bool IsNotificationAboutIgnoredEvent => Event_Type is not (ReepayEvents.InvoiceSettled or
        ReepayEvents.InvoiceRefund or
        ReepayEvents.InvoiceFailed);

    private NotEmptyString GetPspTransactionId()
        => Transaction.IsEmpty() ? new NotEmptyString(EmptyTransactionIdentifier) : new NotEmptyString(Transaction);
}
