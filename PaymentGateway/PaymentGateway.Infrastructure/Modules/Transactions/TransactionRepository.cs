// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Infrastructure.DataAccess.MongoDb;

namespace PaymentGateway.Infrastructure.Modules.Transactions;

/// <inheritdoc cref="ITransactionRepository"/>
public class TransactionRepository : ITransactionRepository
{
    private readonly Func<Transaction, FilterDefinition<Transaction>> _getFilterById = preauthTransaction =>
        new FilterDefinitionBuilder<Transaction>().Eq(transaction => transaction.Id, preauthTransaction.Id);

    private readonly IMongoCollection<Transaction> _transactionCollection;
    private readonly IMongoCollection<BsonDocument> _transactionCollectionAsBson;

    public TransactionRepository(IMongoContext mongoContext)
    {
        _transactionCollection = mongoContext.GetCollection<Transaction>();
        _transactionCollectionAsBson = mongoContext.GetCollection<BsonDocument>(nameof(Transaction));
    }
    
    public async Task<Transaction> InsertAsync(Transaction transaction)
    {
        await _transactionCollection.InsertOneAsync(transaction);
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        await _transactionCollection.ReplaceOneAsync(_getFilterById(transaction), transaction);
        return transaction;
    }

    public Task<Transaction> GetByBillwerkTransactionIdAsync(BillwerkTransactionId id) =>
        _transactionCollection.AsQueryable().SingleAsync(transaction => transaction.BillwerkTransactionId == id);
    
    public async Task<Transaction> GetSingleByPspTransactionIdAsync(NotEmptyString id) => 
        (await _transactionCollectionAsBson.AsQueryable()
            .SingleAsync(transaction => transaction[nameof(Transaction.PspTransactionId)] == id.Value))
            .To(transaction => BsonSerializer.Deserialize<Transaction>(transaction));

    public IEnumerable<Transaction> GetTransactionsByPspTransactionId(NotEmptyString id)
    {
        var documents = _transactionCollectionAsBson.AsQueryable().Where(transaction => transaction[nameof(Transaction.PspTransactionId)] == id.Value).ToList();
        return documents.Select(document => BsonSerializer.Deserialize<Transaction>(document));
    }

    public async Task<Transaction?> GetInitialTransactionByPspAgreementIdAsync(NotEmptyString pspAgreementId)
    {
        var id = pspAgreementId.Value;
        var transaction = await _transactionCollectionAsBson.AsQueryable()
            .Where(x => x[nameof(PreauthTransaction.IsInitial)] == true)
            .SingleOrDefaultAsync(x => x[nameof(Agreement)][nameof(Agreement.PspAgreementId)] == id);

        return transaction != null ? BsonSerializer.Deserialize<Transaction>(transaction) : null;
    }
}
