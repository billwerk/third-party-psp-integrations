// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Webhook;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.BillwerkSDK.Enums;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace FakeProvider.Notifications;

/// <summary>
/// Fake provider notification model.
/// For simplifying purpose copied from billwerk webhook dto: (see <see cref="IntegrationWebhookDto"/>.
/// </summary>
public class FakeProviderNotification : ITypedNotification
{
    /// <summary>
    /// Id of transaction in billwerk system for which event occured.
    /// </summary>
    public string TransactionId { get; set; }

    /// <summary>
    /// Type of payment notification.
    /// </summary>
    public WebhookType Type { get; set;}

    public FakeProviderNotificationStatus Status { get; set; }

    public TransactionNotification ToTransactionNotification(string rawData) => new(PaymentProvider.FakeProvider,
        new NotEmptyString(rawData),
        this,
        new NotEmptyString(TransactionId));
}

/// <summary>
/// Status of transaction connected with notification.
/// Usually, it always make sense to make one more request to PSP side to achieve current actual state of
/// transaction from notification. This enum needed for simplifying approach for Fake Provider (no real PSP calls).
/// </summary>
public enum FakeProviderNotificationStatus
{
    Success = 1,
    Fail = 2,
}
