using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState.BusinessRules;

public class NewStateMustBeAfterCurrentState<TState> : IBusinessRule
    where TState : TransactionStateBase
{
    private readonly TransactionStateCollection<TState> _stateCollection;
    private readonly TState _newState;

    public NewStateMustBeAfterCurrentState(TransactionStateCollection<TState> stateCollection, TState newState)
    {
        _stateCollection = stateCollection;
        _newState = newState;
    }

    public bool IsBroken() => _newState.LastSeenAt <= _stateCollection.Current.LastSeenAt;

    public string Message => "New item can't be less or equal last of inserted.";
}
