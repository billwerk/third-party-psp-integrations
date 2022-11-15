// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// State of the transaction https://reference.reepay.com/api/#transaction
/// </summary>
public enum TransactionState
{
    /// <summary>
    /// The transaction is still processing
    /// </summary>
    [EnumMember(Value = "processing")]
    Processing,

    /// <summary>
    /// The transaction is authorized
    /// </summary>
    [EnumMember(Value = "authorized")]
    Authorized,

    /// <summary>
    /// The transaction is settled
    /// </summary>
    [EnumMember(Value = "settled")]
    Settled,

    /// <summary>
    /// The transaction is refunded
    /// </summary>
    [EnumMember(Value = "refunded")]
    Refunded,

    /// <summary>
    /// The transaction has failed
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed,

    /// <summary>
    /// The transaction has been cancelled
    /// </summary>
    [EnumMember(Value = "cancelled")]
    Cancelled,
}
