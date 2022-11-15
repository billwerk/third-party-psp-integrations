using DryIoc;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Infrastructure.InMemoryStorage.Repositories;

namespace PaymentGateway.Infrastructure.Modules;

public static class SettingsContainer
{
    public static IRegistrator Settings(this IRegistrator registrator, bool useInMemoryStorage)
    {
        if (useInMemoryStorage)
            registrator.Register<ISettingsRepository, PspSettingsMemoryRepository>();
        else
            registrator.Register<ISettingsRepository, SettingsRepository>();

        return registrator;
    }
}
