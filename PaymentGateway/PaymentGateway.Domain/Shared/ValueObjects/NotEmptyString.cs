using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;

public record NotEmptyString : ValueObject<string>
{
    public NotEmptyString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectException<NotEmptyString>("The value was supposed to be a not empty string");

        Value = value;
    }
    
    public static implicit operator string(NotEmptyString notEmptyString) => notEmptyString.Value;
    public static explicit operator NotEmptyString(string value) => new(value);
    public override string ToString() => Value;
}
