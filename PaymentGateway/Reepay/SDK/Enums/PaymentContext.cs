// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

public enum PaymentContext
{
    /// <summary>
    /// customer initiated
    /// </summary>
    [EnumMember(Value = "cit")]
    CIt,
    /// <summary>
    /// merchant initiated
    /// </summary>
    [EnumMember(Value = "mit")]
    MIT,
    /// <summary>
    /// customer initiated using stored information
    /// </summary>
    [EnumMember(Value = "cit_cof")]
    CitCof,
}
