using System.Collections;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Modules.Transactions.Refund;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.Abstraction;
using PaymentGateway.Domain.Modules.Transactions.TransactionState.BusinessRules;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;
using PaymentGateway.Domain.Shared.BusinessRules;

namespace PaymentGateway.Domain.Modules.Transactions.TransactionState;

public class TransactionStateCollection<TState> : ITransactionStateCollection<TState>
    where TState : TransactionStateBase
{
    private readonly ICollection<TState> _innerSequence;

    public TransactionStateCollection(TState state) : this() => _innerSequence.Add(state);

    private TransactionStateCollection() =>
        _innerSequence = new SortedSet<TState>(Comparer<TransactionStateBase>.Create((x, y) => Math.Sign((x.LastSeenAt - y.LastSeenAt).Ticks)));

    public void Add(TState state)
    {
        CheckRule(new CurrentTransactionStateMustBeNonFinal<TState>(this));
        CheckRule(new NewStateMustBeAfterCurrentState<TState>(this, state));
        
        Action validationStepForSpecificState = state switch 
        {
            PreauthTransactionState => () => CheckRule(new NewStateMustBeConsistent<TState, PreauthTransactionState>(this)),
            PaymentTransactionState => () => CheckRule(new NewStateMustBeConsistent<TState, PaymentTransactionState>(this)),
            RefundTransactionState => () => CheckRule(new NewStateMustBeConsistent<TState, RefundTransactionState>(this)),
            TransactionErrorState => () => { },
            var _ => throw new ArgumentOutOfRangeException(nameof(state), state, $"Unsupported transaction state {state.GetType().Name}"),
        };

        validationStepForSpecificState();

        var currentState = Current;
        if (currentState.EqualsTo(state)) currentState.LastSeenAt = state.ReceivedAt;
        else _innerSequence.Add(state);
    }

    public TState Current => _innerSequence.Last();

    public IEnumerator<TState> GetEnumerator() => _innerSequence.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _innerSequence.Count;
    
    private static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
            throw new BusinessRuleValidationException(rule);
    }
}
