// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// https://reference.reepay.com/api/#the-payment-method-object
/// </summary>
public enum PaymentMethodState
{
    [EnumMember(Value = "active")]
    Active,

    /// <summary>
    /// A payment method can be inactivated. An inactivated payment method will not be used to pay invoices. See https://reference.reepay.com/api/#inactivate-payment-method
    /// </summary>
    [EnumMember(Value = "inactivated")]
    Inactivated,
    [EnumMember(Value = "failed")]
    Failed,
    [EnumMember(Value = "pending")]
    Pending,

    /// <summary>
    /// A payment method can be deleted. An deleted payment method will not be used to pay invoices and will not appear in customer payment method lists. For payment methods like MobilePay Subscriptions the subscription agreement will be cancelled.
    /// See https://reference.reepay.com/api/#delete-payment-method
    /// </summary>
    [EnumMember(Value = "deleted")]
    Deleted,
}
