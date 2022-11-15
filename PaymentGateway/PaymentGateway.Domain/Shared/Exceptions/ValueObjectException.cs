// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Shared.Exceptions;

public class ValueObjectException<T> : ArgumentException, IValueObjectException where T : IValueObject
{
    private const string ErrorMessage = "{0} has invalid value. Details: {1}";
    private static string GetErrorMessage(string typeName, string details) => string.Format(ErrorMessage, typeName, details);
    
    public ValueObjectException() { }

    public ValueObjectException(string message) : base(GetErrorMessage(typeof(T).Name, message)) {}
    
    public ValueObjectException(string message, Exception exception) : base(GetErrorMessage(typeof(T).Name, message), exception) {}
}

/// <b>Marker interface for exception handling purpose only!</b>
public interface IValueObjectException { }
