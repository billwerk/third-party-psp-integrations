using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState.BusinessRules;

public class NewStateMustBeConsistent<TState, TExpectedState> : IBusinessRule
    where TState : TransactionStateBase
{
    private readonly TransactionStateCollection<TState> _stateCollection;

    public NewStateMustBeConsistent(TransactionStateCollection<TState> stateCollection)
    {
        _stateCollection = stateCollection;
    }

    public bool IsBroken() => _stateCollection.Any(state => state is not TExpectedState);

    public string Message => $"Inconsistency of transaction states found while adding {typeof(TState).Name} to transaction state.";
}
