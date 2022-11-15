// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// https://reference.reepay.com/api/#the-refund-object
/// </summary>
public enum RefundType
{
    [EnumMember(Value = "card")]
    Card,
    [EnumMember(Value = "mobilepay")]
    MobilePay,
    [EnumMember(Value = "vipps")]
    Vipps,
    [EnumMember(Value = "swish")]
    Swish,
    [EnumMember(Value = "viabill")]
    ViaBill,
    [EnumMember(Value = "manual")]
    Manual,
    [EnumMember(Value = "applepay")]
    ApplePay,
    [EnumMember(Value = "googlepay")]
    GooglePay,
    [EnumMember(Value = "paypal")]
    PayPal,
    [EnumMember(Value = "klarna_pay_now")]
    KlarnaPayNow,
    [EnumMember(Value = "klarna_pay_later")]
    KlarnaPayLater,
    [EnumMember(Value = "klarna_slice_it")]
    KlarnaSliceIt,
    [EnumMember(Value = "klarna_direct_bank_transfer")]
    KlarnaDirectBankTransfer,
    [EnumMember(Value = "klarna_direct_debit")]
    KlarnaDirectDebit,
    [EnumMember(Value = "resurs")]
    Resurs,
    [EnumMember(Value = "mobilepay_subscriptions")]
    MobilePaySubscriptions,
}
