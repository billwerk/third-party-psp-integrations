// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

public enum ErrorState
{
    /// <summary>
    /// The charge operation has been declined by acquirer or issuer with a soft decline. 
    /// This is ususally due to insufficient funds. Future attempts may succeed. 
    /// It is recommended not to retry more than three times a day.
    /// </summary>
    [EnumMember(Value = "soft_declined")]
    SoftDeclined,
    /// <summary>
    /// The charge operation has been declined by acquirer or issuer. 
    /// The hard decline means that no further attempts should be made using the same payment method. 
    /// All subsequent attempts will fail.
    /// </summary>
    [EnumMember(Value = "hard_declined")]
    HardDeclined,
    /// <summary>
    /// A processing error can happen if something goes wrong at, or in between, any of the parties involved in a transaction. 
    /// A processing error can potentially have resulted in a successful charge, but the result never reaches Reepay. 
    /// E.g. a timeout somewhere in the chain. 
    /// Processing errors leading to transactions actually having been completed without knowing the result is frustrating, 
    /// but luckily quite rare. We recommend to retry later on a processing error but if the error persists for days 
    /// the payment method should be marked as failed.
    /// </summary>
    [EnumMember(Value = "processing_error")]
    ProcessingError,
    [EnumMember(Value = "pending")]
    Pending,
}
