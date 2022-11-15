// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc.ImTools;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Notification.Billwerk;
using PaymentGateway.Application.Notification.EntryPoint;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.Types.Transaction;

public class TransactionNotificationHandler : TypedNotificationHandlerBase
{
    public override Type NotificationType => typeof(TransactionNotification);

    private readonly IEnumerable<IPspNotificationHandler> _pspNotificationHandlers;

    private IPspNotificationHandler GetPspNotificationHandler(PaymentProvider paymentProvider) =>
        _pspNotificationHandlers.Single(x => x.PaymentProvider == paymentProvider);

    public TransactionNotificationHandler(
        ITransactionRepository transactionRepository,
        IBillwerkWebhookWrapper billwerkWebhookWrapper,
        IEnumerable<IPspNotificationHandler> pspNotificationHandlers) : base(transactionRepository, billwerkWebhookWrapper)
    {
        _pspNotificationHandlers = pspNotificationHandlers;
    }

    protected override async Task HandleTypedNotificationAsyncInternal<T>(T notification)
    {
        var transactionNotification = notification.To<TransactionNotification>();

        var actualTransactionLinkedToPspTransaction = new SinglePspTransaction(TransactionRepository, transactionNotification.PspTransactionId, transactionNotification.PaymentProvider)
            .ActualTransaction()!;

        var pspHandlingResult = await GetPspNotificationHandler(transactionNotification.PaymentProvider)
            .HandleTransactionNotificationAsync(transactionNotification, actualTransactionLinkedToPspTransaction);

        var postPspHandleTask = pspHandlingResult switch
        {
            IgnoredNotificationHandlingResult ignoredResult => LogIgnoredTransactionNotificationAsync(transactionNotification, ignoredResult),
            NotificationHandlingResult notificationHandlingResult => UpdateTransactionStateAndSendWebhookToBillwerkAsync(notificationHandlingResult, actualTransactionLinkedToPspTransaction),
            _ => throw new ArgumentOutOfRangeException(nameof(pspHandlingResult), $"Not expected type of PSP handling result: {pspHandlingResult.GetType()}. Notification: {notification.ToString()}"),
        };

        await postPspHandleTask;
    }

    private async Task UpdateTransactionStateAndSendWebhookToBillwerkAsync(NotificationHandlingResult notificationHandlingResult, Domain.Modules.Transactions.Transaction actualTransactionLinkedToPspTransaction)
    {
        var targetTransaction = await ChangeTargetTransactionByNotificationResultAsync(actualTransactionLinkedToPspTransaction, notificationHandlingResult);

        TransactionStateBase newStateToAdd = notificationHandlingResult.Result switch
        {
            { IsSuccess: true, Data: PreauthResponseDto preauthResponseDto } => PreauthStateBuilder.Build(preauthResponseDto),
            { IsSuccess: true, Data: PaymentResponseDto paymentResponseDto } => PaymentStateBuilder.Build(targetTransaction.States.Current, paymentResponseDto),
            { IsSuccess: true, Data: RefundResponseDto refundResponseDto } => RefundStateBuilder.Build(refundResponseDto),
            { IsFailure: true, Error: PaymentErrorDto paymentErrorDto } => FailedStateBuilder.Build(paymentErrorDto),
            var _ => throw new NotSupportedException($"Response dto type {notificationHandlingResult.Result.Data.GetType().Name} handling is not supported."),
        };

        targetTransaction.States.Add(newStateToAdd);
        await TransactionRepository.UpdateAsync(targetTransaction);

        if (targetTransaction is RefundTransaction refundTransaction && newStateToAdd is not TransactionErrorState)
            AddConfirmedRefundItemToReferencedPaymentTransaction(refundTransaction, notificationHandlingResult.Result.Data.To<RefundResponseDto>());

        var billwerkWebhook = BillwerkWebhookBuilder.BuildBasedOnTransactionUpdate(targetTransaction, notificationHandlingResult);
        await BillwerkWebhookWrapper.SendWebhookAsync(billwerkWebhook, targetTransaction.WebhookUrl);
    }

    private async void AddConfirmedRefundItemToReferencedPaymentTransaction(RefundTransaction refundTransaction, RefundResponseDto refundResponseDto)
    {
        var referencedPaymentTransaction = (await TransactionRepository.GetByBillwerkTransactionIdAsync(refundTransaction.TargetPaymentTransactionId))
            .To<PaymentTransaction>();
        
        var refundStateForPaymentTransaction = PaymentStateBuilder.BuildStateForRefundUpdate(referencedPaymentTransaction.States.Current, refundResponseDto);
        referencedPaymentTransaction.States.Add(refundStateForPaymentTransaction);

        await TransactionRepository.UpdateAsync(referencedPaymentTransaction);
    }

    private async Task<Domain.Modules.Transactions.Transaction> ChangeTargetTransactionByNotificationResultAsync(Domain.Modules.Transactions.Transaction transaction, NotificationHandlingResult notificationHandlingResult)
        => (transaction, notificationHandlingResult.Result.Data) switch
        {
            (PaymentTransaction, RefundResponseDto refundResponseDto) =>
                (await TransactionRepository.GetSingleByPspTransactionIdAsync(new NotEmptyString(refundResponseDto.PspTransactionId))).To<RefundTransaction>(),
            var _ => transaction,
        };

    private Task LogIgnoredTransactionNotificationAsync(INotification notification, IgnoredNotificationHandlingResult result)
    {
        //TODO: IT-9811: Webhooks logging & mirroring mechanism
        return Task.CompletedTask;
    }
}
