// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

public enum ChargeState
{
    /// <summary>
    /// The charge was created
    /// </summary>
    [EnumMember(Value = "created")]
    Created,
    /// <summary>
    /// The charge went through an authorize and settle flow
    /// </summary>
    [EnumMember(Value = "authorized")]
    Authorized,
    /// <summary>
    /// The charge was settled
    /// </summary>
    [EnumMember(Value = "settled")]
    Settled,
    /// <summary>
    /// Charge failed. To get error codes and additional information use the get charge operation
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed,
    /// <summary>
    /// The charge was cancelled
    /// </summary>
    [EnumMember(Value = "cancelled")]
    Cancelled,
    /// <summary>
    /// Indicates an async processing has been started for an asynchronous payment method. E.g. MobilePay Subscriptions
    /// </summary>
    [EnumMember(Value = "pending")]
    Pending,
}
