// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Domain.Shared.ValueObjects;

public record NonNegativeCount
{
    public int Value { get; private set; }

    public NonNegativeCount(int value)
    {
        Value = value switch
        {
            < 0 => throw new ArgumentException($"Amount can not be negative, but found value: {value}."), 
            var _ => value,
        };
    }

    public static implicit operator int(NonNegativeCount amount) => amount.Value;
    
    public static explicit operator NonNegativeCount(int value) => new(value);
}
