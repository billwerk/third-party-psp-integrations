// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using Reepay.Bearers;
using Reepay.Helpers;

namespace Reepay.SDK.Models.Charges;

/// <summary>
/// https://reference.reepay.com/api/#create-charge
/// </summary>
/// <param name="Handle">Per account unique reference to charge/invoice. E.g. order id from own system. Multiple payments can be attempted for the same handle but only one authorized or settled charge can exist per handle. Max length 255 with allowable characters [a-zA-Z0-9_.-@]. It is recommended to use a maximum of 20 characters as this will allow for the use of handle as reference on bank statements without truncation.</param>
/// <param name="Amount">Amount in the smallest unit. Either amount or order_lines must be provided if charge/invoice does not already exists.</param>
/// <param name="Currency">Optional currency in ISO 4217 three letter alpha code. If not provided the account default currency will be used. The currency of an existing charge or invoice cannot be changed.</param>
/// <param name="Source">The source for the payment. Either an existing payment method for the customer or a card token ct_.... The existing payment method can either be referenced directly with id, e.g. ca_..., or the keyword auto can be given to indicate that the newest active customer payment method should be used.</param>
/// <param name="Settle">Whether or not to immediately settle the charge. If not settled immediately the charge will be authorized and can later be settled. Normally this have to be done within 7 days. The default is not to settle the charge immediately.</param>
/// <param name="CustomerHandle">Customer reference. If charge does not already exist either this reference must be provided, a create customer object must be provided or the source must be a payment method reference (e.g. ca_..) identifying customer. Notice that customer cannot be changed for existing charge/invoice so if handle is provided it must match the customer handle for existing customer.</param>
public record CreateChargeRequest(
    string Handle,
    int Amount,
    string Currency,
    string Source,
    bool Settle,
    string CustomerHandle)
{
    public CreateChargeRequest(PaymentRequestDto paymentRequestDto, Agreement agreement)
        : this(paymentRequestDto.TransactionId,
            paymentRequestDto.RequestedAmount.ToReepayAmount(paymentRequestDto.Currency),
            paymentRequestDto.Currency,
            agreement.PaymentBearer[nameof(CreditCardPaymentBearer.RecurringPaymentMethod)],
            true,
            agreement.PaymentBearer[nameof(CreditCardPaymentBearer.Customer)])
    {
    }

    public CreateChargeRequest(PreauthRequestDto preauthRequestDto, Agreement agreement)
        : this(preauthRequestDto.TransactionId,
            preauthRequestDto.RequestedAmount.ToReepayAmount(preauthRequestDto.Currency),
            preauthRequestDto.Currency,
            agreement.PaymentBearer[nameof(CreditCardPaymentBearer.RecurringPaymentMethod)],
            false,
            agreement.PaymentBearer[nameof(CreditCardPaymentBearer.Customer)])
    {
    }
}
