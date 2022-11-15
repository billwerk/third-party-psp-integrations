// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using PaymentGateway.Shared;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Notification.Types.Transaction;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Helpers;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.Invoices;

/// <summary>
/// https://reference.reepay.com/api/#transaction
/// </summary>
/// <param name="Id">Transaction id assigned by Reepay</param>
/// <param name="State">State of the transaction, one of the following: pending, processing, authorized, settled, refunded, failed, cancelled</param>
/// <param name="Invoice">Invoice handle</param>
/// <param name="Type">Transaction type, one of the following: settle, refund, authorization</param>
/// <param name="Amount">The transaction amount</param>
/// <param name="Settled">When the transaction was settled</param>
/// <param name="Authorized">When the transaction was authorized</param>
/// <param name="Failed">When the transaction failed</param>
/// <param name="Refunded">When the transaction was refunded</param>
/// <param name="Created">Date when the transaction was created</param>
/// <param name="PaymentType">Payment type for transaction</param>
/// <param name="CardTransaction">CardTransaction</param>
public record InvoiceTransaction(
    string Id,
    TransactionState State,
    string Invoice,
    TransactionType Type,
    int Amount,
    DateTime? Settled,
    DateTime? Authorized,
    DateTime? Failed,
    DateTime? Refunded,
    DateTime Created,
    string PaymentType,
    InvoiceCardTransaction? CardTransaction)
{
    public NotificationHandlingResult GetCurrentState(Currency currency) => State switch
    {
        TransactionState.Settled or TransactionState.Processing or TransactionState.Authorized or TransactionState.Refunded =>
            ToSuccessTransactionResponse(currency)
                .To(Result<ITransactionResponseDto, PaymentErrorDto>.Ok)
                .To(result => new NotificationHandlingResult(result)),

        TransactionState.Failed or TransactionState.Cancelled => ToFailedTransactionResponse()
            .To(result => new NotificationHandlingResult(result)),

        _ => throw new ArgumentOutOfRangeException(nameof(State), $"Reepay invoice transaction state {State} is not supported."),
    };

    private ITransactionResponseDto ToSuccessTransactionResponse(Currency currency) => Type switch
    {
        TransactionType.Settle => new PaymentResponseDto
        {
            RequestedAmount = Amount.FromReepayAmount(currency.Value),
            LastUpdated = DateTime.UtcNow,
            PspTransactionId = Id,
            Payments = new List<PaymentItemDto>
            {
                new()
                {
                    Amount = Amount.FromReepayAmount(currency.Value),
                    BookingDate = LocalDate.FromDateTime(Settled!.Value),
                },
            },
            Status = State.ToPaymentTransactionStatus(),
        },

        TransactionType.Refund => new RefundResponseDto
        {
            RefundedAmount = Amount.FromReepayAmount(currency.Value),
            PspTransactionId = Id,
            LastUpdated = DateTime.UtcNow,
            BookingDate = LocalDate.FromDateTime(Refunded!.Value),
            Status = State.ToPaymentTransactionStatus(),
        },

        _ => throw new ArgumentOutOfRangeException(nameof(Type), $"Reepay invoice transaction type {Type} is not supported."),
    };

    private PaymentErrorDto ToFailedTransactionResponse() => ToPaymentErrorDto(State.ToPaymentTransactionStatus(), CardTransaction?.Error, CardTransaction?.ErrorState);
}
