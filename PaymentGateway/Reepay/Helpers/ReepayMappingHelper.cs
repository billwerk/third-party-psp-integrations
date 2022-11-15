// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules.PSP.Enums;
using PaymentGateway.Shared;
using Reepay.SDK.Enums;
using Reepay.SDK.Models.Refund;

namespace Reepay.Helpers;

public static class ReepayMappingHelper
{
    /// <remarks>
    /// see https://en.wikipedia.org/wiki/List_of_circulating_currencies and note that Reepay handles this list incorrectly
    /// </remarks>
    private static int MinimalUnitAmountFor(string currency) =>
        currency switch
        {
            "BHD" or "IQD" or "KWD" or "LYD" or "OMR" or "TND" or "VND" => 1000,
            _ => 100,
        };

    /// <summary>
    /// Convert decimal amount to integral considering currency minimal unit
    /// </summary>
    /// <param name="amount">amount to convert</param>
    /// <param name="currency">currency to consider</param>
    /// <returns></returns>
    public static int ToReepayAmount(this decimal amount, string currency) =>
        Convert.ToInt32(amount * MinimalUnitAmountFor(currency));

    /// <summary>
    /// Convert Reepay integral amount to decimal considering currency minimal unit
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="currency"></param>
    /// <returns></returns>
    public static decimal FromReepayAmount(this int amount, string currency) =>
        Convert.ToDecimal(amount) / MinimalUnitAmountFor(currency);

    /// <summary>
    /// Gives Reepay supported locales given the customer language https://docs.reepay.com/reference/checkout-languages
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public static string LocaleFromLanguage(string language) => language switch
    {
        "en" => "en_US", // English
        "da" => "da_DK", // Danish
        "sv" => "sv_SE", // Swedish
        "no" => "no_NO", // Norwegian Nynorsk
        "nb" => "nb_NO", // Norwegian Bokmål
        "fi" => "fi_FI", // Finish
        "is" => "is_IS", // Icelandic
        "de" => "de_DE", // German
        "es" => "es_ES", // Spanish
        "fr" => "fr_FR", // French
        "it" => "it_IT", // Italian
        "nl" => "nl_NL", // Dutch
        "pl" => "pl_PL", // Polish
        "hu" => "hu_HU", // Hungarian
        "ro" => "ro_RO", // Romanian
        _ => "en_US",
    };

    public static PaymentMethod[] ToPaymentMethods(this CreditCardTypes[]? types)
    {
        var paymentMethods =
            (types ?? Array.Empty<CreditCardTypes>())
            .Select<CreditCardTypes, PaymentMethod?>(type => type switch
            {
                CreditCardTypes.Visa => PaymentMethod.Visa,
                CreditCardTypes.Mastercard => PaymentMethod.MasterCard,
                CreditCardTypes.Amex => PaymentMethod.AmericanExpress,
                CreditCardTypes.UnionPay => PaymentMethod.ChinaUnionPay,
                CreditCardTypes.JCB => PaymentMethod.JCB,
                CreditCardTypes.Discover => PaymentMethod.Discover,
                CreditCardTypes.Diners => PaymentMethod.Diners,
                CreditCardTypes.Dankort => PaymentMethod.Dankort,
                _ => null,
            })
            .Where(e => e.HasValue).Select(e => e!.Value).ToArray();

        return paymentMethods.Length > 0 ? paymentMethods : new[] { PaymentMethod.Card };
    }

    public static PaymentTransactionStatus ToPaymentTransactionStatus(this ChargeState state) => state switch
    {
        ChargeState.Created => PaymentTransactionStatus.Initiated,
        ChargeState.Authorized => PaymentTransactionStatus.Pending,
        ChargeState.Settled => PaymentTransactionStatus.Succeeded,
        ChargeState.Failed => PaymentTransactionStatus.Failed,
        ChargeState.Cancelled => PaymentTransactionStatus.Cancelled,
        ChargeState.Pending => PaymentTransactionStatus.Pending,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
    };

    /// <summary>
    /// Builds <see cref="RefundResponseDto"/> based on <see cref="RefundRequestDto"/> and <see cref="RefundResponse"/>
    /// </summary>
    /// <param name="refundRequestDto">Request for refund operation</param>
    /// <param name="response">Refund creation response from Reepay</param>
    /// <returns>For succeeded refund <see cref="RefundResponseDto"/> result, for failed refund <see cref="PaymentErrorDto"/> result</returns>
    public static Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto> CreateRefundResponseDtoFromReepayResponse(RefundRequestDto refundRequestDto, RefundResponse response)
        => IsRefundFailed(response)
            ? CreateRefundFailureResultWithResponseDto()
            : response.ToRefundResponse(refundRequestDto);

    /// <summary>
    /// Maps Reepay refund status <see cref="RefundState"/> to billwerk transaction status <see cref="PaymentTransactionStatus"/>
    /// </summary>
    /// <param name="refundState"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static PaymentTransactionStatus ToPaymentTransactionStatus(this RefundState refundState) => refundState switch
    {
        RefundState.Refunded => PaymentTransactionStatus.Succeeded,
        RefundState.Processing => PaymentTransactionStatus.Pending,
        RefundState.Failed => PaymentTransactionStatus.Failed,
        var _ => throw new ArgumentOutOfRangeException(nameof(refundState), refundState, null),
    };

    private static bool IsRefundFailed(RefundResponse refundResponse) =>
        refundResponse.State.ToPaymentTransactionStatus() == PaymentTransactionStatus.Failed;

    private static Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto> CreateRefundFailureResultWithResponseDto() =>
        new PaymentErrorDto
        {
            ReceivedAt = DateTime.UtcNow,
            ErrorMessage = "Refund failed after processing on Reepay side.",
            Status = PaymentTransactionStatus.Failed,
        };

    public static PaymentTransactionStatus ToPaymentTransactionStatus(this TransactionState state) => state switch
    {
        TransactionState.Authorized => PaymentTransactionStatus.Succeeded,
        TransactionState.Cancelled => PaymentTransactionStatus.Cancelled,
        TransactionState.Processing => PaymentTransactionStatus.Pending,
        TransactionState.Settled => PaymentTransactionStatus.Succeeded,
        TransactionState.Refunded => PaymentTransactionStatus.Succeeded,
        TransactionState.Failed => PaymentTransactionStatus.Failed,
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
    };

    public static PaymentErrorCode ToPaymentErrorCode(this ReepayErrorCode code) => code switch
    {
        ReepayErrorCode.InvalidRequest or
            ReepayErrorCode.DuplicateHandle or
            ReepayErrorCode.NoOrderLinesForInvoice => PaymentErrorCode.InvalidData,

        ReepayErrorCode.InternalError or
            ReepayErrorCode.NotImplemented or
            ReepayErrorCode.QueryTookTooLong => PaymentErrorCode.InternalProviderError,

        ReepayErrorCode.InvalidUserOrPassword or
            ReepayErrorCode.NoAccountsForUser or
            ReepayErrorCode.UnknownAccount or
            ReepayErrorCode.UserBlockedDueToFailedLogins => PaymentErrorCode.LoginError,

        ReepayErrorCode.NotAuthenticated or
            ReepayErrorCode.Unauthorized => PaymentErrorCode.PermissionDenied,

        ReepayErrorCode.TestDataNotAllowedForLiveAccount or
            ReepayErrorCode.LiveDataNotAllowedForTestAccount or
            ReepayErrorCode.CardGatewayStateLiveTestMustMatchAccountStateLiveTest => PaymentErrorCode.InvalidConfiguration,

        ReepayErrorCode.MissingAmount or
            ReepayErrorCode.RefundAmountTooHigh or
            ReepayErrorCode.InvoiceHasZeroAmount or
            ReepayErrorCode.AmountHigherThanAuthorizedAmount or
            ReepayErrorCode.CreditAmountTooHigh or
            ReepayErrorCode.NonPositiveAmount => PaymentErrorCode.InvalidAmount,

        ReepayErrorCode.TransactionDeclined => PaymentErrorCode.Rejected,

        ReepayErrorCode.RequestRateLimitExceeded or
            ReepayErrorCode.ConcurrentRequestLimitExceeded => PaymentErrorCode.RateLimit,

        ReepayErrorCode.CurrencyNotSupportedByPaymentMethod => PaymentErrorCode.InvalidCurrency,

        _ => PaymentErrorCode.UnmappedError,
    };

    public static PaymentErrorDto ToPaymentErrorDto(PaymentTransactionStatus paymentTransactionStatus, TransactionError? transactionError, ErrorState? errorState) => new()
    {
        ReceivedAt = DateTime.UtcNow,
        Status = paymentTransactionStatus,
        PspErrorCode = transactionError?.ToString() ?? "",
        ErrorMessage = $"{transactionError} {errorState}",
        ErrorCode = transactionError?.ToPaymentErrorCode() ?? PaymentErrorCode.UnmappedError,
    };

    private static PaymentErrorCode ToPaymentErrorCode(this TransactionError code) => code switch
    {
        TransactionError.CreditCardExpired => PaymentErrorCode.BearerExpired,

        TransactionError.AuthorizationExpired => PaymentErrorCode.InvalidPreconditions,

        // Rejected: caused by acquirer
        TransactionError.DeclinedByAcquirer or
            TransactionError.AcquirerRejectedError or
            TransactionError.AcquirerCommunicationError or
            TransactionError.AcquirerError or
            TransactionError.AcquirerIntegrationError or
            TransactionError.ScaRequired or
            TransactionError.SettleBlocked => PaymentErrorCode.Rejected,

        TransactionError.CreditCardLostOrStolen or
            TransactionError.CreditCardSuspectedFraud or
            TransactionError.FraudBlock or
            TransactionError.AcquirerAuthenticationError => PaymentErrorCode.BearerInvalid,

        TransactionError.RefundAmountTooHigh or
            TransactionError.AuthorizationAmountExceeded => PaymentErrorCode.InvalidAmount,

        // Cancelled: caused by Reepay system or by merchant
        TransactionError.AuthorizationVoided or
            TransactionError.RiskFilterBlock => PaymentErrorCode.Canceled,

        TransactionError.AcquirerConfigurationError => PaymentErrorCode.InvalidConfiguration,

        TransactionError.InsufficientFunds => PaymentErrorCode.InsufficientBalance,

        _ => PaymentErrorCode.UnmappedError,
    };
}
