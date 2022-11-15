// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Wrapper;

namespace Reepay.Notifications;

public class ReepayNotificationHandler : IPspNotificationHandler
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IFlurlClientFactory _flurlClientFactory;

    private const string SkipReasonForFailedFetch = "Fetch request was not successful for pspTransactionId: {0} , Notification Id {1} . Details: {2}";
    private const string SkipReasonForFailedVerification = "Signature verification failed for Reepay notification! Notification Id : {0} , ReepaySettingsId : {1}";
    private const string SkipReasonForNotRelevantEvent = "Event {0} shouldn't be handled. Notification Id : {1} , ReepaySettingsId : {2} Skipped.";

    public PaymentProvider PaymentProvider => PaymentProvider.Reepay;

    public ReepayNotificationHandler(ISettingsRepository settingsRepository, IFlurlClientFactory flurlClientFactory)
    {
        _settingsRepository = settingsRepository;
        _flurlClientFactory = flurlClientFactory;
    }

    /// <summary>
    /// Reepay notification handling details:
    /// 1. Handle only 4 invoice-transaction-related events (settled, failed, refund-success, refund-failed);
    /// 2. Only verified notifications are handled.
    /// 3. Based on provided data, Reepay transaction was fetched and mapped to the common state model. We don't rely on events inside notifications
    /// due to consistency reasons plus we for failed cases we need more info.
    /// </summary>
    public Task<ITransactionNotificationHandlingResult> HandleTransactionNotificationAsync(TransactionNotification transactionNotification, Transaction transaction)
    {
        var (reepaySettings, reepayNotification, wrapper) = SetupHandlingProcess(transactionNotification, transaction);

        if (reepayNotification.IsNotificationAboutIgnoredEvent)
            return BuildResponseForNotRelevantEvent(reepayNotification, reepaySettings);

        return ReepayNotificationVerifier.Verify(reepayNotification, in reepaySettings) switch
        {
            true => HandleVerifiedTransactionNotification(reepayNotification, transaction, wrapper),
            false => BuildResponseForFailedVerification(reepayNotification, reepaySettings),
        };
    }

    private async Task<ITransactionNotificationHandlingResult> HandleVerifiedTransactionNotification(
        ReepayNotification reepayNotification,
        Transaction transaction,
        ReepayWrapper wrapper)
    {
        var invoiceTransactionAsResult = await wrapper.GetInvoiceTransaction(reepayNotification.Invoice, reepayNotification.Transaction);

        return invoiceTransactionAsResult.IsSuccess switch
        {
            true => invoiceTransactionAsResult.Data.GetCurrentState(transaction.Currency),
            false => BuildIgnoreResponseForFailedFetch(invoiceTransactionAsResult.Error, reepayNotification),
        };
    }

    private (ReepaySettings, ReepayNotification, ReepayWrapper) SetupHandlingProcess(TransactionNotification notification, Transaction transaction)
    {
        var reepaySettings = _settingsRepository.GetById(transaction.PspSettingsId).To<ReepaySettings>();
        var reepayNotification = notification.TypedNotification.To<ReepayNotification>();

        var wrapper = new ReepayWrapper(reepaySettings, _flurlClientFactory);

        return (reepaySettings, reepayNotification, wrapper);
    }

    private static Task<ITransactionNotificationHandlingResult> BuildResponseForNotRelevantEvent(ReepayNotification reepayNotification, ReepaySettings reepaySettings) =>
        SkipReasonForNotRelevantEvent.Format(reepayNotification.Event_Type, reepayNotification.Id, reepaySettings.Id.ToString())
            .To(s => new NotEmptyString(s))
            .To(s => new IgnoredNotificationHandlingResult(s))
            .To(Task.FromResult<ITransactionNotificationHandlingResult>);

    private Task<ITransactionNotificationHandlingResult> BuildResponseForFailedVerification(ReepayNotification reepayNotification, ReepaySettings reepaySettings) =>
        SkipReasonForFailedVerification.Format(reepayNotification.Id, reepaySettings.Id.ToString())
            .To(s => new NotEmptyString(s))
            .To(s => new IgnoredNotificationHandlingResult(s))
            .To(Task.FromResult<ITransactionNotificationHandlingResult>);

    private IgnoredNotificationHandlingResult BuildIgnoreResponseForFailedFetch(PaymentErrorDto paymentErrorDto, ReepayNotification reepayNotification) =>
        SkipReasonForFailedFetch.Format(reepayNotification.Transaction, reepayNotification.Id, paymentErrorDto.ToString())
            .To(s => new NotEmptyString(s))
            .To(s => new IgnoredNotificationHandlingResult(s));
}
