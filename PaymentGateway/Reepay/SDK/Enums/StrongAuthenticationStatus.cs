// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// Status for strong customer authentication. See https://reference.reepay.com/api/#cardv2
/// </summary>
public enum StrongAuthenticationStatus
{
    /// <summary>
    /// 3D Secure authenticated
    /// </summary>
    [EnumMember(Value = "threed_secure")]
    ThreeDSecure,

    /// <summary>
    /// 3D Secure authentication not performed as card not enrolled
    /// </summary>
    [EnumMember(Value = "threed_secure_not_enrolled")]
    ThreeDSecureNotEnrolled,

    /// <summary>
    /// Secure by Nets authenticated
    /// </summary>
    [EnumMember(Value = "secured_by_nets")]
    SecuredByNets,
}
