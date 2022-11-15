// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// https://reference.reepay.com/api/#the-refund-object
/// </summary>
public enum RefundState
{
    [EnumMember(Value = "refunded")]
    Refunded,
    [EnumMember(Value = "failed")]
    Failed,
    [EnumMember(Value = "processing")]
    Processing,
}
