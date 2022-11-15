// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// https://reference.reepay.com/api/#cardv2
/// </summary>
public enum CardType
{
    [EnumMember(Value = "unknown")]
    Unknown,
    [EnumMember(Value = "visa")]
    Visa,
    [EnumMember(Value = "mc")]
    MasterCard,
    [EnumMember(Value = "dankort")]
    Dankort,
    [EnumMember(Value = "visa_dk")]
    VisaDankort,
    [EnumMember(Value = "ffk")]
    Forbrugsforeningen,
    [EnumMember(Value = "visa_elec")]
    VisaElectron,
    [EnumMember(Value = "maestro")]
    Maestro,
    [EnumMember(Value = "laser")]
    Laser,
    [EnumMember(Value = "amex")]
    AmericanExpress,
    [EnumMember(Value = "diners")]
    Diners,
    [EnumMember(Value = "discover")]
    Discover,
    [EnumMember(Value = "jcb")]
    JCB,
}
