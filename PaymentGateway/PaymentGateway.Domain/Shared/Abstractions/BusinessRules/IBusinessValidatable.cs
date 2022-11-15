// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Shared.BusinessRules;

namespace PaymentGateway.Domain.Shared.Abstractions.BusinessRules;


public abstract class BusinessValidatableBase
{
    protected void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}
