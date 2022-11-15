// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.SDK.Models.Charges;

/// <summary>
/// https://reference.reepay.com/api/#chargesource
/// </summary>
/// <param name="Type">Type of charge source: card - existing customer card, card_token - card token, mpo - MobilePay Online, vipps, swish, viabill, manual, applepay, googlepay, paypal, klarna_pay_now, klarna_pay_later, klarna_slice_it, klarna_direct_bank_transfer, klarna_direct_debit, resurs, ideal, p24, blik or mobilepay_subscriptions</param>
/// <param name="Card">Reference to customer card if source type card</param>
/// <param name="Mps">Reference to MobilePay Subscriptions payment method if source type mobilepay_subscriptions</param>
/// <param name="Fingerprint">Uniquely identifies this particular card number if credit card source</param>
/// <param name="Provider">Card acquirer or card payment gateway used if card source: reepay, clearhaus, nets, swedbank, handelsbanken, elavon, bambora, valitor, dibs, stripe, epay, test</param>
/// <param name="AuthTransaction">Reference to authorization transaction if charge is settled after authorization</param>
/// <param name="CardType">Card type if credit card source: unknown, visa, mc, dankort, visa_dk, ffk, visa_elec, maestro, laser, amex, diners, discover or jcb</param>
/// <param name="TransactionCardType">Transaction card type if credit card source. Will differ from card_type if co-branded card. Transaction card type is the card type used for the transaction. unknown, visa, mc, dankort, visa_dk, ffk, visa_elec, maestro, laser, amex, diners, discover or jcb</param>
/// <param name="ExpDate">Card expire date on form MM-YY if credit card source</param>
/// <param name="MaskedCard">Masked card number if credit card source</param>
/// <param name="CardCountry">Card issuing country if credit card source, in ISO 3166-1 alpha-2</param>
/// <param name="StrongAuthenticationStatus">Status for strong customer authentication: threed_secure - 3D Secure authenticated, threed_secure_not_enrolled - 3D Secure authentication not performed as card not enrolled, secured_by_nets - Secure by Nets authenticated</param>
/// <param name="ThreeDSecureStatus">If 3D Secure authenticated the 3D status will either be Y (fully authenticated) or A (attempted authenticated). An attempted authentication means that card issuer (e.g. bank) does not support 3D Secure so no full authentication has been performed. Attempted authentication normally means liability shift, but this can differ between acquirers.</param>
/// <param name="RiskRule">If this parameter is set the charge has either been flagged or declined by a Reepay Risk Filter rule. For flag action rules the charge can be successful, but may require special attention. For block action rules the decline error will be risk_filter_block.</param>
/// <param name="AcquirerCode">Card acquirer error code in case of card error</param>
/// <param name="AcquirerMessage">Acquirer message in case of error</param>
/// <param name="AcquirerReference">Card acquirer reference to transaction in case of card source. E.g. Nets order id or Clearhaus reference.</param>
/// <param name="TextOnStatement">Resulting text on bank statement if known</param>
/// <param name="SurchargeFee">Potential card surcharge fee added to amount if surcharging enabled</param>
public record ChargeSource(
    string Type,
    string Card,
    string Mps,
    string Fingerprint,
    string Provider,
    string AuthTransaction,
    string CardType,
    string TransactionCardType,
    string ExpDate,
    string MaskedCard,
    string CardCountry,
    string StrongAuthenticationStatus,
    string ThreeDSecureStatus,
    string RiskRule,
    string AcquirerCode,
    string AcquirerMessage,
    string AcquirerReference,
    string TextOnStatement,
    int SurchargeFee);
