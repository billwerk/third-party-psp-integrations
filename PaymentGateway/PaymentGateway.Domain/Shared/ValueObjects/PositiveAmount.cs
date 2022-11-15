using PaymentGateway.Domain.Shared.Exceptions;

namespace PaymentGateway.Domain.Shared.ValueObjects;

public record PositiveAmount : ValueObject<decimal>
{
    public PositiveAmount(decimal value)
    {
        Value = value switch
        {
            0 => throw new ValueObjectException<PositiveAmount>("Amount must be greater than zero."),
            < 0 => throw new ValueObjectException<PositiveAmount>($"Amount '{value}' must not be negative."), 
            var _ => value,
        };
    }

    public PositiveAmount(string value)
    {
        var isParseSuccessful = decimal.TryParse(value, out var parsedValue);
        Value = parsedValue switch
        {
            var _ when !isParseSuccessful => throw new ValueObjectException<PositiveAmount>($"String '{value}' has valid decimal format."),
            0 => throw new ValueObjectException<PositiveAmount>("Amount must be greater than zero."),
            < 0 => throw new ValueObjectException<PositiveAmount>($"Amount '{parsedValue}' must not be negative."), 
            var _ => parsedValue,
        };
    }
    
    public static implicit operator decimal(PositiveAmount positiveAmount) => positiveAmount.Value;
    public static explicit operator PositiveAmount(decimal value) => new(value);
}
