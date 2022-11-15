// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Shared;
using Reepay.Bearers;
using Reepay.Helpers;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.Charges;

/// <summary>
/// An abstraction on top of invoices, providing a convenient way to charge customers with one-time payments 
/// and to pay existing unpaid invoices. See https://reference.reepay.com/api/#the-charge-object
/// </summary>
/// <param name="Handle">Per account unique reference to charge/invoice. E.g. order id from own system. 
/// Multiple payments can be attempted for the same handle but only one succeeded charge can exist per handle. 
/// Max length 255 with allowable characters [a-zA-Z0-9_.-@].</param>
/// <param name="State">The charge state one of the following: created, authorized, settled, failed, cancelled, pending. 
/// A pending state after create charge indicates an async processing has been started for an asynchronous payment method. 
/// E.g. MobilePay Subscriptions. See also processing. The result of the charge will be delivered in webhook as 
/// either invoice_authorized, invoice_settled or invoice_failed.</param>
/// <param name="Customer">Customer handle</param>
/// <param name="Amount">The invoice amount including VAT</param>
/// <param name="Currency">Currency for the account in ISO 4217 three letter alpha code</param>
/// <param name="Authorized">When the charge was authorized, if the charge went through an authorize and settle flow, in ISO-8601 extended offset date-time format.</param>
/// <param name="Settled">When the charge was settled, in ISO-8601 extended offset date-time format.</param>
/// <param name="Cancelled">When the charge was cancelled, in ISO-8601 extended offset date-time format.</param>
/// <param name="Created">When the invoice was created, in ISO-8601 extended offset date-time format.</param>
/// <param name="Transaction">Transaction id assigned by Reepay. Assigned when transaction is performed.</param>
/// <param name="Error">Reepay error code if failed. See transaction errors.</param>
/// <param name="Processing">For asynchronous payment methods this flag indicates that the charge is awaiting result. The charge/invoice state will be pending.</param>
/// <param name="Source">Object describing the source for the charge. E.g. credit card.</param>
/// <param name="RefundedAmount">Refunded amount</param>
/// <param name="AuthorizedAmount">Authorized amount if authorization was performed. The maximum amount that can be settled.</param>
/// <param name="ErrorState">Reepay error state if failed: soft_declined, hard_declined or processing_error. Soft and hard declines indicate a card decline. A soft decline is possibly recoverable and a subsequent request with the same card may succeed. E.g. insufficient funds. A processing error indicates an error processing the card either at Reepay, the acquirer, or between Reepay and the acquirer.</param>
/// <param name="RecurringPaymentMethod">Optional reference to recurring payment method created in conjunction with charging</param>
/// <param name="PaymentContext">Payment context describing if the transaction is customer or merchant initiated, one of the following values: cit, mit, cit_cof</param>
public record Charge(
    string Handle,
    ChargeState State,
    string Customer,
    int Amount,
    string Currency,
    DateTime? Authorized,
    DateTime? Settled,
    DateTime? Cancelled,
    DateTime Created,
    string Transaction,
    TransactionError? Error,
    bool Processing,
    ChargeSource Source,
    int RefundedAmount,
    int AuthorizedAmount,
    ErrorState? ErrorState,
    string RecurringPaymentMethod,
    PaymentContext PaymentContext)
{
    public Result<InitialResponse, PaymentErrorDto> ToPreauthResponse(string transactionId) =>
        IsChargeFailed()
            ? CreatePaymentErrorDto()
            : CreatePreauthSuccessfulResponse(transactionId);

    public Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto> ToPaymentResponse(string transactionId, Agreement? agreement) =>
        IsChargeFailed()
            ? CreatePaymentErrorDto()
            : CreatePaymentSuccessfulResponse(transactionId, agreement);

    private bool IsChargeFailed() => State.ToPaymentTransactionStatus() == PaymentTransactionStatus.Failed;

    private InitialResponse CreatePreauthSuccessfulResponse(string transactionId) => new(new PreauthResponseDto
        {
            RequestedAmount = Amount.FromReepayAmount(Currency),
            Currency = Currency,
            AuthorizedAmount = AuthorizedAmount.FromReepayAmount(Currency),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Status = PaymentTransactionStatus.Succeeded,
            TransactionId = transactionId,
            LastUpdated = DateTime.UtcNow,
            PspTransactionId = Transaction,
            Bearer = new CreditCardPaymentBearer(this).ToDictionary(),
        },
        null,
        BookingDate: null,
        new Dictionary<string, string>
        {
            [ReepayConstants.PspTransactionDataInvoiceHandle] = Handle,
        });

    private ExtendedResponse<PaymentResponseDto> CreatePaymentSuccessfulResponse(string transactionId, Agreement? agreement) =>
        new(new PaymentResponseDto
            {
                Bearer = agreement?.PaymentBearer ?? new Dictionary<string, string>(),
                Currency = Currency,
                ExternalTransactionId = transactionId,
                LastUpdated = DateTime.UtcNow,
                Payments = new List<PaymentItemDto>
                {
                    new()
                    {
                        Amount = Amount.FromReepayAmount(Currency),
                        BookingDate = LocalDate.FromDateTime(Settled ?? Authorized ?? Created),
                    },
                },
                PspTransactionId = Transaction,
                RefundedAmount = RefundedAmount.FromReepayAmount(Currency),
                RequestedAmount = Amount.FromReepayAmount(Currency),
                Status = State.ToPaymentTransactionStatus(),
                TransactionId = transactionId,
            },
                new Dictionary<string, string>
            {
                [ReepayConstants.PspTransactionDataInvoiceHandle] = Handle,
            });

    private PaymentErrorDto CreatePaymentErrorDto() => ToPaymentErrorDto(State.ToPaymentTransactionStatus(), Error, ErrorState);
}
