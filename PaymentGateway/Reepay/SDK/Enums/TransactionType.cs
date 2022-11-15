// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// Transaction type https://reference.reepay.com/api/#transaction
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Settlement transaction
    /// </summary>
    [EnumMember(Value = "settle")]
    Settle,

    /// <summary>
    /// Refund transaction
    /// </summary>
    [EnumMember(Value = "refund")]
    Refund,

    /// <summary>
    /// Authorization transaction
    /// </summary>
    [EnumMember(Value = "authorization")]
    Authorization,
}
