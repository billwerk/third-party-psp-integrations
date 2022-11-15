// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Domain.Shared.ValueObjects;

public interface IValueObject { }

public abstract record ValueObject<T> : IValueObject
{
 #pragma warning disable CS8618
    public T Value { get; protected init; }
 #pragma warning restore CS8618
}
