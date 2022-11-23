﻿using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;

namespace PaymentGateway.Domain.Shared.BusinessRules;

public class BusinessRuleValidationException : Exception
{
    public IBusinessRule BrokenRule { get; }

    public string Details { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    public override string ToString() => $"{BrokenRule.GetType().FullName}: {BrokenRule.Message}";
}