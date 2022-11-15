// Copyright (c) billwerk GmbH. All rights reserved

namespace Billwerk.Payment.SDK.Enums
{
    public enum PaymentErrorCode
    {
        UnmappedError = 0,
        LimitExceeded = 1,
        BearerInvalid = 2,
        BearerExpired = 3,
        InvalidCountry = 4,
        InvalidAmount = 5,
        InvalidCurrency = 6,
        LoginError = 7, // WARNING!!! Use this only for merchant PSP account error. Raising this error causes PSP to be disabled
        InvalidData = 8, //More general than InvalidPaymentData from frontend side
        InsufficientBalance = 9,
        AlreadyExecuted = 10,
        InvalidPreconditions = 11,
        InternalError = 12,
        InternalProviderError = 13,
        RateLimit = 14,
        InvalidConfiguration = 15,
        PermissionDenied = 16, // WARNING!!! Use this only for merchant PSP account error. Raising this error causes PSP to be disabled
        Canceled = 17,
        Rejected = 18,
        PSPConnectionProblem = 19,
        InvalidBic = 20,
        InvalidIBAN = 21,
        PSPConnectionTimeout = 22,
        InvalidNationalIdNumber = 23,
        FailedCustomerUpdate = 24, // Failed customer data update required in PayExInvoice payment transactions
    }
}
