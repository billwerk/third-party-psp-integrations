// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Newtonsoft.Json.Converters;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using Reepay.Helpers;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.ChargeSession;

/// <summary>
/// A really slim model with the minimum set of fields required to create a charge session. See https://docs.reepay.com/reference/createchargesession
/// </summary>
/// <param name="Locale">Optional locale for session. E.g. en_GB, da_DK, es_ES. Defaults to configuration locale or account locale.</param>
/// <param name="AcceptUrl">If checkout is opened in separate window the customer will be directed to this page after success.t</param>
/// <param name="CancelUrl">If checkout is opened in separate window the customer will be directed to this page if the customer cancels.</param>
/// <param name="Order">Object defining order details such as amount, currency and order text</param>
/// <param name="Recurring">If set a recurring payment method is stored for the customer and a reference returned. This parameter if set to true will limit payment methods to those that are reusable. Cannot be used in conjunction with recurring_optional.</param>
/// <param name="Settle">Whether or not to immediately settle (capture). Default is false. If not settled immediately an authorization will be performed which can be settled later. Normally this have to be done within 7 days.</param>
public record CreateChargeSessionRequest(
    string Locale,
    string AcceptUrl,
    string CancelUrl,
    ChargeSessionRequestOrder Order,
    PaymentMethod[]? PaymentMethods,
    bool Settle = false,
    bool Recurring = true)
{
    public CreateChargeSessionRequest(PreauthRequestDto preauthRequestDto, PaymentMethod[]? paymentMethods, bool settle = false, bool recurring = true)
        : this(LocaleFromLanguage(preauthRequestDto.PayerData.Language),
            preauthRequestDto.PaymentMeansReference.SuccessReturnUrl,
            preauthRequestDto.PaymentMeansReference.AbortReturnUrl,
            new ChargeSessionRequestOrder(preauthRequestDto.TransactionId,
                preauthRequestDto.RequestedAmount.ToReepayAmount(preauthRequestDto.Currency),
                preauthRequestDto.Currency,
                preauthRequestDto.OrderData.ExternalContractId,
                new ChargeSessionRequestCustomer(preauthRequestDto.PayerData.EmailAddress,
                    preauthRequestDto.PayerData.FirstName,
                    preauthRequestDto.PayerData.LastName,
                    preauthRequestDto.TransactionId)),
            paymentMethods,
            settle,
            recurring)
    {
    }
}
