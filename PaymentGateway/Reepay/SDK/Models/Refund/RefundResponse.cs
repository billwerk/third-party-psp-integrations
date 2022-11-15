// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Shared;
using Reepay.Helpers;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.Refund;

/// <summary>
/// https://reference.reepay.com/api/#the-refund-object
/// </summary>
/// <param name="Id">Refund id assigned by Reepay. Required</param>
/// <param name="State">The refund state either refunded, failed or processing. The processing state can only be returned for asynchronous payment method (not card). Required</param>
/// <param name="Invoice">Invoice/charge handle. Required</param>
/// <param name="Amount">Refunded amount. Required</param>
/// <param name="Currency">Currency for the account in ISO 4217 three letter alpha code. Required</param>
/// <param name="Transaction">Transaction id assigned by Reepay. Required</param>
/// <param name="Error">Reepay error code if failed. See https://reference.reepay.com/api/#transaction-errors. Optional</param>
/// <param name="Type">Type of refund, either card, mobilepay, vipps, swish, viabill, manual, applepay, googlepay, paypal, klarna_pay_now, klarna_pay_later, klarna_slice_it, klarna_direct_bank_transfer, klarna_direct_debit, resurs or mobilepay_subscriptions. Required</param>
/// <param name="Created">When the refund was created, in ISO-8601 extended offset date-time format. Required</param>
/// <param name="CreditNoteId">Credit note id for successful refund. Optional</param>
/// <param name="RefTransaction">Id of a possible settled transaction that has been refunded. Optional</param>
/// <param name="ErrorState">Reepay error state if failed: hard_declined or processing_error. A hard decline indicates a refund decline by acquirer. A processing error indicates an error processing the refund either at Reepay, the acquirer, or between Reepay and the acquirer. Optional</param>
/// <param name="AcquirerMessage">Acquirer message in case of error. Optional</param>
public record RefundResponse(
    string Id,
    RefundState State,
    string Invoice,
    int Amount,
    string Currency,
    string Transaction,
    TransactionError? Error,
    RefundType Type,
    DateTime Created,
    string CreditNoteId,
    string RefTransaction,
    ErrorState? ErrorState,
    string AcquirerMessage)
{
    /// <summary>
    /// Builds <see cref="RefundResponseDto"/> based on <see cref="RefundRequestDto"/> and current <see cref="RefundResponse"/>
    /// </summary>
    /// <param name="refundRequestDto">Request from billwerk side for refund operation</param>
    /// <returns></returns>
    public Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto> ToRefundResponse(RefundRequestDto refundRequestDto)
        => IsRefundFailed()
            ? CreateRefundPaymentErrorDto()
            : CreateRefundSuccessfulResponse(refundRequestDto);

    private bool IsRefundFailed() => State.ToPaymentTransactionStatus() == PaymentTransactionStatus.Failed;

    private ExtendedResponse<RefundResponseDto> CreateRefundSuccessfulResponse(RefundRequestDto refundRequestDto) =>
        new(new RefundResponseDto
            {
                RefundedAmount = Amount.FromReepayAmount(refundRequestDto.Currency),
                BookingDate = LocalDate.FromDateTime(Created),
                Currency = Currency,
                Status = State.ToPaymentTransactionStatus(),
                LastUpdated = DateTime.UtcNow,
                PspTransactionId = Id,
                RequestedAmount = refundRequestDto.RequestedAmount,
                TransactionId = refundRequestDto.TransactionId,
            },
            new Dictionary<string, string>
            {
                [ReepayConstants.PspTransactionDataInvoiceHandle] = Invoice,
            });

    private PaymentErrorDto CreateRefundPaymentErrorDto() => ToPaymentErrorDto(State.ToPaymentTransactionStatus(), Error, ErrorState);
}

