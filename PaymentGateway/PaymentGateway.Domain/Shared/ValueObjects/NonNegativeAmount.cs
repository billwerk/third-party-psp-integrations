using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;

public record NonNegativeAmount : ValueObject<decimal>
{
    public NonNegativeAmount(decimal value) =>
        Value = value switch
        {
            < 0 => throw new ValueObjectException<NonNegativeAmount>($"Amount '{value}' must not be negative."), 
            var _ => value,
        };

    public NonNegativeAmount(string value)
    {
        var isParseSuccessful = decimal.TryParse(value, out var parsedValue);
        Value = parsedValue switch
        {
            var _ when !isParseSuccessful => throw new ValueObjectException<NonNegativeAmount>($"String '{value}' has invalid decimal format."),
            < 0 => throw new ValueObjectException<NonNegativeAmount>($"Amount '{parsedValue}' must not be negative."), 
            var _ => parsedValue,
        };
    }
    
    public static implicit operator decimal(NonNegativeAmount amount) => amount.Value;
    public static explicit operator NonNegativeAmount(decimal value) => new(value);
}
