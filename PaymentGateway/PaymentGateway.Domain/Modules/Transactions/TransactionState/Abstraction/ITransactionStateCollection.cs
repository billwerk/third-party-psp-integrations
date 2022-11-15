namespace PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;

public interface ITransactionStateCollection<TState> : IReadOnlyCollection<TState> where TState : TransactionStateBase
{
    TState Current { get; }
    
    void Add(TState state);
}
