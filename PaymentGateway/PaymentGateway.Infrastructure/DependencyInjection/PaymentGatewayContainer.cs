using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using FakeProvider;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Application;
using PaymentGateway.Application.Modules.Settings;
using PaymentGateway.Application.Notification.Billwerk;
using PaymentGateway.Infrastructure.DataAccess;
using PaymentGateway.Infrastructure.Modules;
using PaymentGateway.Infrastructure.Modules.Transactions;
using PaymentGateway.Shared;
using Reepay;

namespace PaymentGateway.Infrastructure.DependencyInjection;

public static class PaymentGatewayContainer
{
    private const string PlaygroundEnvironmentUseInMemoryStorage = "PlaygroundEnvironment:UseInMemoryStorage";
    
    public static IContainer Build(IConfiguration configuration)
    {
        var container = CreateDefaultContainer();
        
        container.Register<IFlurlClientFactory, PspFlurlClientFactory>(Reuse.Scoped);
        container.Register<IBillwerkWebhookWrapper, BillwerkWebhookWrapper>(Reuse.Scoped);
        container.Register<ISettingsHandler, SettingsHandler>(Reuse.Scoped);

        var useInMemoryStorage = configuration[PlaygroundEnvironmentUseInMemoryStorage]!.To(Convert.ToBoolean);
        
        return container.Database(configuration)
            .Transaction(useInMemoryStorage)
            .Notification()
            .Settings(useInMemoryStorage)
            .Reepay()
            .FakeProvider()
            .To<IContainer>();
    }
    
    private static IContainer CreateDefaultContainer() =>
        new Container(cfg => cfg.WithDefaultReuse(Reuse.ScopedOrSingleton)).WithDependencyInjectionAdapter();
}
