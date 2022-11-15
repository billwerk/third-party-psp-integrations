// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Notification.Types.Transaction.TransactionStateBuilders;

public static class PaymentStateBuilder
{
    public static PaymentTransactionState Build(TransactionStateBase state, PaymentResponseDto responseDto)
    {
        var currentState = state.To<PaymentTransactionState>();
        
        var oldPaymentItems = currentState.Payments;
        var oldChargebackItems = currentState.Chargebacks;
        var paymentItems = oldPaymentItems.Concat(PaymentItemsFromResponseDto(responseDto).SkipSubsequentEqualsAndTakeRemaining(oldPaymentItems)).ToList();
        var chargebackItems = oldChargebackItems.Concat(ChargebackItemsFromResponseDto(responseDto).SkipSubsequentEqualsAndTakeRemaining(oldChargebackItems)).ToList();

        return new PaymentTransactionState(responseDto.Status, responseDto.LastUpdated, paymentItems, currentState.Refunds, chargebackItems);
    }

    public static PaymentTransactionState BuildStateForRefundUpdate(TransactionStateBase state, RefundResponseDto refundResponseDto)
    {
        var currentState = state.To<PaymentTransactionState>();
        
        var oldRefundItems = currentState.Refunds;
        var refundItems = currentState.Refunds.Concat(new[] { RefundItemFromRefundResponseDto(refundResponseDto) }.SkipSubsequentEqualsAndTakeRemaining(oldRefundItems)).ToList();
        
        return new PaymentTransactionState(currentState.Status, DateTime.UtcNow, currentState.Payments, refundItems, currentState.Chargebacks);
    }
    
    private static IEnumerable<PaymentItem> PaymentItemsFromResponseDto(PaymentResponseDto responseDto) =>
        responseDto.Payments.Select(itemDto => new PaymentItem
        {
            PspItemId = itemDto.ExternalItemId,
            Description = itemDto.Description,
            BookingDate = itemDto.BookingDate,
            PositiveAmount = new PositiveAmount(itemDto.Amount),
        });

    private static IEnumerable<ChargebackItem> ChargebackItemsFromResponseDto(PaymentResponseDto responseDto) =>
        responseDto.Chargebacks.OrEmpty()
            .Select(itemDto => new ChargebackItem
            {
                PspItemId = itemDto.ExternalItemId,
                Description = itemDto.Description,
                BookingDate = itemDto.BookingDate,
                Amount = itemDto.Amount,
                Reason = itemDto.Reason,
                FeeAmount = itemDto.FeeAmount,
                PspReasonCode = itemDto.PspReasonCode,
                PspReasonMessage = itemDto.PspReasonMessage,
            });
    
    private static RefundItem RefundItemFromRefundResponseDto(RefundResponseDto refundResponseDto) => new()
    {
        BookingDate = refundResponseDto.BookingDate,
        Description = refundResponseDto.Description,
        PositiveAmount = new PositiveAmount(refundResponseDto.RefundedAmount),
        PspItemId = refundResponseDto.PspTransactionId,
    };
}
