using Billwerk.Payment.SDK.Enums;

namespace PaymentGateway.Domain.Modules.Transactions;

public record PaymentMethodInfo(PaymentProviderRole PaymentProviderRole, PaymentProvider PaymentProvider)
{
    public PaymentProviderRole PaymentProviderRole { get; private set; } = PaymentProviderRole;

    public PaymentProvider PaymentProvider { get; private set; } = PaymentProvider;

    public override string ToString() => $"{PaymentProvider}-{PaymentProviderRole}";
}
