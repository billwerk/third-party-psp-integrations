// Copyright (c) billwerk GmbH. All rights reserved

using Reepay.SDK.Enums;

namespace Reepay.SDK.Models.PaymentMethods;

/// <summary>
/// https://reference.reepay.com/api/#cardv2
/// </summary>
/// <param name="Gateway">Card gateway tied to card</param>
/// <param name="Fingerprint">Uniquely identifies this particular card number</param>
/// <param name="Reactivated">Date and time of reactivation if the card has been reactivated from failed state. In ISO-8601 extended offset date-time format.</param>
/// <param name="GwRef">Card gateway reference id</param>
/// <param name="CardType">Card type: unknown, visa, mc, dankort, visa_dk, ffk, visa_elec, maestro, laser, amex, diners, discover or jcb</param>
/// <param name="TransactionCardType">Card type used in authentication and the card type used for subsequent MIT transactions. Will differ from card_type if co-branded card. unknown, visa, mc, dankort, visa_dk, ffk, visa_elec, maestro, laser, amex, diners, discover or jcb</param>
/// <param name="CardAgreement">Card agreement id</param>
/// <param name="ExpDate">Card expire date on form MM-YY</param>
/// <param name="MaskedCard">Masked card number</param>
/// <param name="LastSuccess">Date and time of last succesfull use of the card. In ISO-8601 extended offset date-time format.</param>
/// <param name="LastFailed">Date and time of last failed use of the card. In ISO-8601 extended offset date-time format.</param>
/// <param name="FirstFail">Date and time of first succesfull use of the card. In ISO-8601 extended offset date-time format.</param>
/// <param name="ErrorCode">An error code from the last failed use of the card. See transaction errors.</param>
/// <param name="ErrorState">Error state from last failed use of the card: pending, soft_declined, hard_declined or processing_error</param>
/// <param name="StrongAuthenticationStatus">Status for strong customer authentication: threed_secure - 3D Secure authenticated, threed_secure_not_enrolled - 3D Secure authentication not performed as card not enrolled, secured_by_nets - Secure by Nets authenticated</param>
/// <param name="ThreeDSecureStatus">If 3D Secure authenticated the 3D status will either be Y (fully authenticated) or A (attempted authenticated). An attempted authentication means that card issuer (e.g. bank) does not support 3D Secure so no full authentication has been performed. Attempted authentication normally means liability shift, but this can differ between acquirers.</param>
/// <param name="RiskRule">If this parameter is set the card has been flagged by Reepay Risk Filter with a flag rule. Special attention may be required before using the card for recurring payments or subscription sign-up.</param>
/// <param name="CardCountry">Card issuing country in ISO 3166-1 alpha-2</param>
public record PaymentMethodCard(
    string Gateway,
    string? Fingerprint,
    DateTime? Reactivated,
    string GwRef,
    CardType CardType,
    CardType TransactionCardType,
    string CardAgreement,
    string? ExpDate,
    string? MaskedCard,
    DateTime? LastSuccess,
    DateTime? LastFailed,
    DateTime? FirstFail,
    TransactionError? ErrorCode,
    ErrorState? ErrorState,
    StrongAuthenticationStatus StrongAuthenticationStatus,
    ThreeDSecureStatus ThreeDSecureStatus,
    string? RiskRule,
    string? CardCountry);
