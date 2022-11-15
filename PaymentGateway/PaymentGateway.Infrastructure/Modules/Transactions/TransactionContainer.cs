// Copyright (c) billwerk GmbH. All rights reserved

using DryIoc;
using PaymentGateway.Domain.Modules.Transactions;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Infrastructure.InMemoryStorage.Repositories;

namespace PaymentGateway.Infrastructure.Modules.Transactions;

public static class TransactionContainer
{
    public static IRegistrator Transaction(this IRegistrator registrator, bool useInMemoryStorage)
    {
        if (useInMemoryStorage)
        {
            registrator.Register<ITransactionRepository, TransactionInMemoryRepository>();
            registrator.Register<IAgreementRepository, AgreementInMemoryRepository>();
        }
        else
        {
            registrator.Register<ITransactionRepository, TransactionRepository>();
            registrator.Register<IAgreementRepository, AgreementRepository>();   
        }

        return registrator;
    }
}
