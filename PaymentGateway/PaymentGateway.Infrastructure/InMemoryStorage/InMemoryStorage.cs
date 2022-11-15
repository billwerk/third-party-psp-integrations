// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Modules.Transactions;

namespace PaymentGateway.Infrastructure.InMemoryStorage;

public static class InMemoryStorage
{
    static InMemoryStorage()
    {
        Transactions = new List<Transaction>();
        PspSettings = new List<PspSettings>();
    }

    public static IList<Transaction> Transactions { get; }
    
    public static IList<PspSettings> PspSettings { get; }
}
