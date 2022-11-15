using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState.BusinessRules;

public class CurrentTransactionStateMustBeNonFinal<TState> : IBusinessRule
    where TState : TransactionStateBase
{
    private readonly TransactionStateCollection<TState> _stateCollection;

    public CurrentTransactionStateMustBeNonFinal(TransactionStateCollection<TState> stateCollection)
    {
        _stateCollection = stateCollection;
    }

    public bool IsBroken() => _stateCollection.Current is TransactionErrorState ||
                              _stateCollection.Current.Status is PaymentTransactionStatus.Cancelled;

    public string Message => "Operations for transaction state collection are blocked.";
}
