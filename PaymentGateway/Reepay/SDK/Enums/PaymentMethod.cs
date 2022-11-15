// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Reepay.SDK.Enums;

/// <summary>
/// Checkout Payment Methods
/// https://docs.reepay.com/reference/checkout-payment-methods
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum PaymentMethod
{
    [EnumMember(Value = "card")]
    Card, // All available debit / credit cards
    [EnumMember(Value = "dankort")]
    Dankort, // Dankort
    [EnumMember(Value = "visa")]
    Visa, // VISA
    [EnumMember(Value = "visa_elec")]
    VisaElectron, // VISA Electron
    [EnumMember(Value = "mc")]
    MasterCard, // MasterCard
    [EnumMember(Value = "amex")]
    AmericanExpress, // American Express
    [EnumMember(Value = "mobilepay")]
    MobilePay, // MobilePay Online
    [EnumMember(Value = "mobilepay_subscriptions")]
    MobilePaySubscriptions, // MobilePay Subscriptions
    [EnumMember(Value = "viabill")]
    ViaBill, // ViaBill
    [EnumMember(Value = "resurs")]
    Resurs, // Resurs Bank
    [EnumMember(Value = "swish")]
    Swish, // Swish
    [EnumMember(Value = "vipps")]
    Vipps, // Vipps
    [EnumMember(Value = "diners")]
    Diners, // Diners Club
    [EnumMember(Value = "maestro")]
    Maestro, // Maestro
    [EnumMember(Value = "laser")]
    Laser, // Laser
    [EnumMember(Value = "discover")]
    Discover, // Discover
    [EnumMember(Value = "jcb")]
    JCB, // JCB
    [EnumMember(Value = "china_union_pay")]
    ChinaUnionPay, // China Union Pay
    [EnumMember(Value = "ffk")]
    Ffk, // Forbrugsforeningen
    [EnumMember(Value = "paypal")]
    PayPal, // PayPal
    [EnumMember(Value = "applepay")]
    ApplePay, // Apple Pay
    [EnumMember(Value = "googlepay")]
    GooglePay, // Google Pay
    [EnumMember(Value = "klarna_pay_later")]
    KlarnaPayLater, // Klarna Pay Later
    [EnumMember(Value = "klarna_pay_now")]
    KlarnaPayNow, // Klarna Pay Now
    [EnumMember(Value = "klarna_slice_it")]
    KlarnaSliceIt, // Klarna Slice It!
    [EnumMember(Value = "klarna_direct_bank_transfer")]
    KlarnaDirectBankTransfer, // Klarna Direct Bank Transfer
    [EnumMember(Value = "klarna_direct_debit")]
    KlarnaDirectDebit, // Klarna Direct Debit
    [EnumMember(Value = "ideal")]
    Ideal, // iDEAL
    [EnumMember(Value = "blik")]
    Blik, // BLIK
    [EnumMember(Value = "p24")]
    P24, // Przelewy24 (P24)
    [EnumMember(Value = "verkkopankki")]
    Verkkopankki, // Verkkopankki
    [EnumMember(Value = "giropay")]
    GiroPay, // giropay
    [EnumMember(Value = "sepa")]
    Sepa, // SEPA Direct Debit
}
