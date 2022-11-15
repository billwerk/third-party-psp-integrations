// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Notification.PSP;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.BillwerkSDK.Enums;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Notifications;

namespace FakeProvider.Notifications;

public class FakeProviderNotificationHandler : IPspNotificationHandler
{
    public PaymentProvider PaymentProvider => PaymentProvider.FakeProvider;
    public Task<ITransactionNotificationHandlingResult> HandleTransactionNotificationAsync(
        TransactionNotification transactionNotification,
        Transaction transaction)
    {
        var fakeProviderNotification = transactionNotification.TypedNotification.To<FakeProviderNotification>();

        ITransactionNotificationHandlingResult handlingResult = (fakeProviderNotification.Status) switch
        {
            FakeProviderNotificationStatus.Success =>
                Result<ITransactionResponseDto, PaymentErrorDto>.Ok(BuildSuccessNotificationHandlingDto(transaction, fakeProviderNotification))
                    .To(result => new NotificationHandlingResult(result)),

            FakeProviderNotificationStatus.Fail =>
                Result<ITransactionResponseDto, PaymentErrorDto>.Failure(BuildFailNotificationHandlingDto())
                    .To(result => new NotificationHandlingResult(result)),
            
            _ => new IgnoredNotificationHandlingResult(new NotEmptyString("Unknown value in notification status field. Ignored.")),
        };

        return handlingResult.To(Task.FromResult);
    }

   /// <summary>
   /// For real example all fields should be filled from PSP response.
   /// Fake Provider simplified a lot. Check <see cref="ReepayNotificationHandler"/> for more real example.
   /// </summary>
   private ITransactionResponseDto BuildSuccessNotificationHandlingDto(
        Transaction transaction,
        FakeProviderNotification fakeProviderNotification)
        => fakeProviderNotification.Type switch
        {
            WebhookType.Payment => new PaymentResponseDto
            {
                Status = PaymentTransactionStatus.Succeeded,
                Payments = new PaymentItemDto
                    {
                        Description = "Payment success",
                        Amount = (transaction as PaymentTransaction).RequestedAmount.Value,
                        BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
                    }.One()
                    .ToList(),
                LastUpdated = DateTime.UtcNow,
            },
            WebhookType.Refund => new RefundResponseDto
            {
                RefundedAmount = (transaction as RefundTransaction).RequestedAmount.Value,
                RequestedAmount = (transaction as RefundTransaction).RequestedAmount,
                Status = PaymentTransactionStatus.Succeeded,
                LastUpdated = DateTime.UtcNow,
                BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
            },
            WebhookType.Preauth => new PreauthResponseDto()
            {
                AuthorizedAmount = (transaction as PreauthTransaction).RequestedAmount.Value,
                Status = PaymentTransactionStatus.Succeeded,
            },
        };
    

   /// <summary>
   /// Return hardcoded error for simplified Fake Provider webhook handling flow.
   /// </summary>
   private PaymentErrorDto BuildFailNotificationHandlingDto() => new()
    {
        ErrorCode = PaymentErrorCode.InternalError,
        ReceivedAt = DateTime.UtcNow,
        ErrorMessage = "Failed webhook notification via Fake Provider (simulation).",
    };

}
