// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.RecurringSession;

/// <summary>
/// https://docs.reepay.com/reference/createrecurringsession
/// </summary>
/// <param name="Locale">Optional locale for session. E.g. en_GB, da_DK, es_ES. Defaults to configuration locale or account locale.</param>
/// <param name="AcceptUrl">If checkout is opened in separate window the customer will be directed to this page after success</param>
/// <param name="CancelUrl">If checkout is opened in separate window the customer will be directed to this page if the customer cancels</param>
/// <param name="Currency">Optional currency to choose acquirer agreement from. Only use this argument if specifically necessary to select agreement based on currency for acquirers not supporting multi-currency.</param>
/// <param name="CreateCustomer">Customer to be created</param>
public record CreateRecurringSessionRequest(
    string Locale,
    string AcceptUrl,
    string CancelUrl,
    string Currency,
    PaymentMethod[]? PaymentMethods,
    RecurringSessionCustomer CreateCustomer)
{
    public CreateRecurringSessionRequest(PreauthRequestDto preauthRequestDto, PaymentMethod[]? paymentMethods)
        : this(LocaleFromLanguage(preauthRequestDto.PayerData.Language),
            preauthRequestDto.PaymentMeansReference.SuccessReturnUrl,
            preauthRequestDto.PaymentMeansReference.AbortReturnUrl,
            preauthRequestDto.Currency,
            paymentMethods,
            new RecurringSessionCustomer(preauthRequestDto.PayerData.EmailAddress,
                preauthRequestDto.PayerData.FirstName,
                preauthRequestDto.PayerData.LastName,
                preauthRequestDto.TransactionId))
    {
    }
}
