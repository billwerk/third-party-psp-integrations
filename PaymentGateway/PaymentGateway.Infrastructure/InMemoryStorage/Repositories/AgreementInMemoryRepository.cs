// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Modules.Transactions.Preauth;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Infrastructure.InMemoryStorage.Repositories;

public class AgreementInMemoryRepository : IAgreementRepository
{
    public Task<Agreement> GetAgreementByIdAsync(AgreementId agreementId) =>
        InMemoryStorage.Transactions
            .Single(tr => tr is PreauthTransaction preauthTransaction &&
                                   preauthTransaction.IsInitial &&
                                   preauthTransaction.Agreement.BillwerkAgreementId == agreementId)
            .To<PreauthTransaction>()
            .Agreement
            .To(Task.FromResult);
}
