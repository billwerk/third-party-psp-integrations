// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.Enums;

namespace PaymentGateway.Application.Modules.Transactions;

public static class ProviderRoleMapper
{
    public static PaymentProviderRole FromPublicToInternal(PublicPaymentProviderRole role) => role switch
    {
        PublicPaymentProviderRole.Default => PaymentProviderRole.Default,
        PublicPaymentProviderRole.CreditCard => PaymentProviderRole.CreditCard,
        PublicPaymentProviderRole.Debit => PaymentProviderRole.Debit,
        PublicPaymentProviderRole.OnAccount => PaymentProviderRole.OnAccount,
        PublicPaymentProviderRole.Betalingsservice => PaymentProviderRole.Betalingsservice,
        PublicPaymentProviderRole.iDEAL => PaymentProviderRole.iDEAL,
        PublicPaymentProviderRole.Autogiro => PaymentProviderRole.Autogiro,
        PublicPaymentProviderRole.Avtalegiro => PaymentProviderRole.Avtalegiro,
        PublicPaymentProviderRole.MobilePay => PaymentProviderRole.BlackLabel,
        PublicPaymentProviderRole.AmazonPay => PaymentProviderRole.BlackLabel,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null),
    };
}
