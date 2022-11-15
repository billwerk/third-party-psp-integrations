// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// https://reference.reepay.com/api/#chargesource
/// </summary>
public enum ThreeDSecureStatus
{
    [EnumMember(Value = "Y")]
    FullyAuthenticated,

    /// <summary>
    /// An attempted authentication means that card issuer (e.g. bank) does not support 3D Secure so no full authentication has been performed. Attempted authentication normally means liability shift, but this can differ between acquirers.
    /// </summary>
    [EnumMember(Value = "A")]
    AttemptedAuthentication,
}
