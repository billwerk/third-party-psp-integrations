// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Infrastructure.InMemoryStorage.Repositories;

public class PspSettingsMemoryRepository : ISettingsRepository
{
    //We don't expect more than one settings for test purpose in memory storage.
    public IMerchantPspSettings GetById(NotEmptyString pspSettingsId)
        => InMemoryStorage.PspSettings.First();

    public IMerchantPspSettings GetDefault()
        => InMemoryStorage.PspSettings.FirstOrDefault();

    public void SaveSettings(PspSettings pspSettings)
    {
        var settings = InMemoryStorage.PspSettings.SingleOrDefault(settings => settings.Id == pspSettings.Id);

        if (settings is not null)
        {
            InMemoryStorage.PspSettings.IndexOf(settings)
                .Do(index => { InMemoryStorage.PspSettings[index] = pspSettings; });
        }
        else
            InMemoryStorage.PspSettings.Add(pspSettings);
    }
}
