// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

public enum InvoiceState
{
    /// <summary>
    /// The invoice was created
    /// </summary>
    [EnumMember(Value = "created")]
    Created,
    /// <summary>
    /// For asynchronous payment methods, e.g. MobilePay subscriptions, this status indicates that an invoice transaction 
    /// is in state processing and is awaiting result.
    /// </summary>
    [EnumMember(Value = "pending")]
    Pending,
    /// <summary>
    /// Dunning for the invoice was started
    /// </summary>
    [EnumMember(Value = "dunning")]
    Dunning,
    /// <summary>
    /// The invoice settled
    /// </summary>
    [EnumMember(Value = "settled")]
    Settled,
    /// <summary>
    /// The invoice was cancelled
    /// </summary>
    [EnumMember(Value = "cancelled")]
    Cancelled,
    /// <summary>
    /// The invoice went through an authorize and settle flow
    /// </summary>
    [EnumMember(Value = "authorized")]
    Authorized,
    /// <summary>
    /// The invoice failed
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed,
}
