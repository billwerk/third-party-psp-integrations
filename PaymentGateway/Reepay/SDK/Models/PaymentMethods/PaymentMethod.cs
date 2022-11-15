// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Bearers;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.PaymentMethods;

/// <summary>
/// https://reference.reepay.com/api/#the-payment-method-object
/// </summary>
/// <param name="Id">Unique id for payment method. Required</param>
/// <param name="State">State of the payment method: active, inactivated, failed, pending or deleted. Required</param>
/// <param name="Customer">Customer by handle. Required</param>
/// <param name="Reference">Optional reference provided when creating the payment method. For payment methods created with Reepay Checkout the reference will correspond to the session id for the Checkout session that created the payment method.</param>
/// <param name="Failed">Date when the payment method failed. In ISO-8601 extended offset date-time format. Optional</param>
/// <param name="Created">Date when the payment method was created. In ISO-8601 extended offset date-time format. Required</param>
/// <param name="Card">Card object in case of card payment method. Optional</param>
/// <param name="PaymentType">Payment type for saved payment method, either: card, mobilepay, vipps, swish, viabill, manual, applepay, googlepay, paypal, klarna_pay_now, klarna_pay_later, klarna_slice_it, klarna_direct_bank_transfer, klarna_direct_debit, resurs or mobilepay_subscriptions. Required</param>
public record PaymentMethod(
    string Id,
    PaymentMethodState State,
    string Customer,
    string Reference,
    DateTime? Failed,
    DateTime Created,
    PaymentMethodCard Card,
    PaymentMethodType PaymentType)
{
    public Result<InitialResponse, PaymentErrorDto> ToPreauthResponse(string transactionId, Currency currency) =>
        IsPaymentMethodFailed()
            ? CreatePaymentErrorDto()
            : new InitialResponse(new PreauthResponseDto
                {
                    RequestedAmount = 0,
                    AuthorizedAmount = 0,
                    Currency = currency.Value,
                    LastUpdated = DateTime.UtcNow,
                    Status = PaymentTransactionStatus.Succeeded,
                    TransactionId = transactionId,
                    Bearer = new CreditCardPaymentBearer(this).ToDictionary(),
                    ExpiresAt = DateTime.MaxValue,
                },
                null);

    private bool IsPaymentMethodFailed() => State == PaymentMethodState.Failed;

    private PaymentErrorDto CreatePaymentErrorDto() => ToPaymentErrorDto(PaymentTransactionStatus.Failed, Card.ErrorCode, Card.ErrorState);
}
