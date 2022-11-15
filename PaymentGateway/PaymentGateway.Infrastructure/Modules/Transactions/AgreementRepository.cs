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

/// <inheritdoc cref="IAgreementRepository"/>
public class AgreementRepository : IAgreementRepository
{
    private readonly IMongoCollection<BsonDocument> _transactionBsonCollection;
    

    public AgreementRepository(IMongoContext mongoContext) => _transactionBsonCollection = mongoContext.GetCollection<BsonDocument>(nameof(Transaction));

    public async Task<Agreement> GetAgreementByIdAsync(AgreementId agreementId) =>
        (await _transactionBsonCollection.AsQueryable()
            .SingleAsync(x => x[nameof(PreauthTransaction.Agreement)][nameof(PreauthTransaction.Agreement.BillwerkAgreementId)] == agreementId.Value &&
                              x[nameof(PreauthTransaction.IsInitial)] == true))
        [nameof(PreauthTransaction.Agreement)].AsBsonDocument
        .To(agreement => BsonSerializer.Deserialize<Agreement>(agreement));
}
