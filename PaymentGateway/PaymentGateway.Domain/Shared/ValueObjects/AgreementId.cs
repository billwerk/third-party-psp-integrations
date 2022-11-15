using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;


public record AgreementId : ValueObject<string>
{
    //In billwerk we don't have now specific format for it. But it must be unique!
    //It comes from ObjectId.
    public AgreementId(string value)
    {
        Value = value switch
        {
            var _ when string.IsNullOrWhiteSpace(value) => throw new ValueObjectException<AgreementId>("Agreement Id must not be null or empty."),
            var _ => value
        };
    }
}
