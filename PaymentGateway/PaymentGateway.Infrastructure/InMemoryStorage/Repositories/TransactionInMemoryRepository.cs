// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Infrastructure.InMemoryStorage.Repositories;

public class TransactionInMemoryRepository : ITransactionRepository
{
    public Task<Transaction> InsertAsync(Transaction transaction)
    {
        InMemoryStorage.Transactions.Add(transaction);
        return Task.FromResult(transaction);
    }

    public Task<Transaction> UpdateAsync(Transaction transaction)
    {
        InMemoryStorage.Transactions
            .Single(tr => tr.Id == transaction.Id)
            .To(tr => InMemoryStorage.Transactions.IndexOf(tr))
            .Do(index => { InMemoryStorage.Transactions[index] = transaction; });

        return Task.FromResult(transaction);
    }

    public Task<Transaction> GetByBillwerkTransactionIdAsync(BillwerkTransactionId id)
        => InMemoryStorage.Transactions.Single(tr => tr.BillwerkTransactionId == id)
            .To(Task.FromResult);
    public Task<Transaction> GetSingleByPspTransactionIdAsync(NotEmptyString id)
        => InMemoryStorage.Transactions.Single(tr => tr.PspTransactionId == id)
            .To(Task.FromResult);

    public Task<Transaction?> GetInitialTransactionByPspAgreementIdAsync(NotEmptyString pspAgreementId)
        => InMemoryStorage.Transactions.SingleOrDefault(tr =>
                tr is PreauthTransaction preauthTransaction && preauthTransaction.Agreement.PspAgreementId == pspAgreementId)
            .To(Task.FromResult);

    public IEnumerable<Transaction> GetTransactionsByPspTransactionId(NotEmptyString id)
        => InMemoryStorage.Transactions.Where(tr => tr.PspTransactionId == id)
            .ToList();
}
