using MongoDB.Bson;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;
using PaymentGateway.Domain.Shared.BusinessRules;

namespace PaymentGateway.Domain.Shared.Abstractions.DataAccess.DatabaseObjects;

public abstract class DatabaseObjectBase : IDatabaseObject<ObjectId>
{
    public ObjectId Id { get; private set; }

    protected DatabaseObjectBase() => ForceId();
    
    private void ForceId()
    {
        if (ObjectId.Empty == Id)
            Id = ObjectId.GenerateNewId();
    }
    
    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}
