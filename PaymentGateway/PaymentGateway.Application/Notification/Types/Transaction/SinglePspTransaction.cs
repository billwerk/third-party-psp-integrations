// Copyright (c) billwerk GmbH. All rights reserved

using System.Collections.Immutable;
using PaymentGateway.Shared;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Payment;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.Abstractions.BusinessRules;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Application.Notification.Types.Transaction;

/// <summary>
/// High-lvl abstraction which cover cases when one PSP-transaction linked with several PG-transactions.
/// Used for notification mechanism, to identify PG-transaction which status should be changed.
/// </summary>
public class SinglePspTransaction : BusinessValidatableBase
{
    private readonly ImmutableList<Domain.Modules.Transactions.Transaction> _referencedTransactions;

    /// <summary>
    /// Latest performed single PG-transaction -> actual one.
    /// </summary>
    /// <returns></returns>
    public Domain.Modules.Transactions.Transaction? ActualTransaction() => _referencedTransactions.MaxBy(tr => tr.Id);

    public SinglePspTransaction(ITransactionRepository transactionRepository, NotEmptyString pspTransactionId, PaymentProvider paymentProvider)
    {
        _referencedTransactions = transactionRepository.GetTransactionsByPspTransactionId(pspTransactionId).ToImmutableList();

        CheckRule(new OnlyPreauthAndCaptureCanBeLinkedToOnePspTransaction(_referencedTransactions, pspTransactionId, paymentProvider));
    }
}

internal class OnlyPreauthAndCaptureCanBeLinkedToOnePspTransaction : IBusinessRule
{
    private readonly ImmutableList<Domain.Modules.Transactions.Transaction> _referencedTransactions;
    private readonly NotEmptyString _pspTransactionId;
    private readonly PaymentProvider _paymentProvider;

    public OnlyPreauthAndCaptureCanBeLinkedToOnePspTransaction(ImmutableList<Domain.Modules.Transactions.Transaction> transactions, NotEmptyString pspTransactionId, PaymentProvider paymentProvider)
    {
        _referencedTransactions = transactions;
        _pspTransactionId = pspTransactionId;
        _paymentProvider = paymentProvider;
    }

    public bool IsBroken() => _referencedTransactions.Count(tr => tr.PspTransactionId == _pspTransactionId) switch
        {
            0 or 1 => false,
            2 => !CheckThatPairContainsPreauthAndCaptureTransaction(),
            _ => true,
        };
    
    private bool CheckThatPairContainsPreauthAndCaptureTransaction()
    {
        var preauthTransaction = _referencedTransactions.SingleOrDefault(tr => tr is PreauthTransaction).To<PreauthTransaction>();
        var captureTransaction = _referencedTransactions.SingleOrDefault(tr => tr is PaymentTransaction).To<PaymentTransaction>();
        
        var isValidPairOfPreauthAndCapture = preauthTransaction is not null && captureTransaction is not null &&
                                             preauthTransaction.PaymentMethodInfo.ToString() == captureTransaction.PaymentMethodInfo.ToString() &&
                                             preauthTransaction.PaymentMethodInfo.PaymentProvider == _paymentProvider && 
                                             preauthTransaction.Id < captureTransaction.Id &&
                                             preauthTransaction.Agreement.BillwerkAgreementId == captureTransaction.AgreementId;

        return isValidPairOfPreauthAndCapture;
    }

    public string Message => $"Inconsistent transactions detected for PSP-transaction {_pspTransactionId}, PSP: {_paymentProvider}!";
}
