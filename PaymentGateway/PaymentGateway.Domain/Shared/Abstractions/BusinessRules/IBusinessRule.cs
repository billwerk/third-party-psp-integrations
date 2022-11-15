namespace PaymentGateway.Domain.Shared.Abstractions.BusinessRules;

public interface IBusinessRule
{
    bool IsBroken();

    string Message { get; }
}
