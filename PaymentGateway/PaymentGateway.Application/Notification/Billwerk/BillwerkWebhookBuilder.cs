// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Webhook;
using DryIoc.ImTools;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.BillwerkSDK.Enums;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;

namespace PaymentGateway.Application.Notification.Billwerk;

public static class BillwerkWebhookBuilder
{

    public static IntegrationWebhookDto BuildBasedOnTransactionUpdate(
        Domain.Modules.Transactions.Transaction transaction,
        NotificationHandlingResult notificationHandlingResult) => new()
    {
        TransactionId = transaction.BillwerkTransactionId.Value,
        Type = notificationHandlingResult.Result.To(result =>
        {
            return result.IsSuccess
                ? result.Data switch
                {
                    PreauthResponseDto => WebhookType.Preauth,
                    PaymentResponseDto => WebhookType.Payment,
                    RefundResponseDto => WebhookType.Refund,
                    _ => throw new ArgumentOutOfRangeException()
                }
                : transaction switch
                {
                    PreauthTransaction => WebhookType.Preauth,
                    PaymentTransaction => WebhookType.Payment,
                    RefundTransaction => WebhookType.Refund,
                    _ => throw new ArgumentOutOfRangeException(nameof(transaction), transaction, null)
                };
        }),
    };
}
