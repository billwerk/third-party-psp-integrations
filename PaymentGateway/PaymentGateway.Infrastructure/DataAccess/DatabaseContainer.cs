using DryIoc;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Domain;
using PaymentGateway.Infrastructure.DataAccess.Database;
using PaymentGateway.Infrastructure.DataAccess.MongoDb;

namespace PaymentGateway.Infrastructure.DataAccess;

public static class DatabaseContainer
{
    private const string AppDatabaseNamePath = "DatabaseSettings:DatabaseName";
    private const string AppDatabaseConnectionPath = "DatabaseSettings:DefaultConnectionString";
    
    private const string PaymentGatewayDomainPath = "PaymentGatewaySettings:Domain";
    private const string PaymentGatewayEnvironmentUrlPath = "PaymentGatewaySettings:EnvironmentUrl";
    private const string PaymentGatewayApiUrlPath = "PaymentGatewaySettings:ApiUrl";

    public static IRegistrator Database(this IRegistrator registrator, IConfiguration configuration)
    {
        registrator.RegisterInstance<IDatabaseSettings>(new DatabaseSettings
        {
            ConnectionString = configuration[AppDatabaseConnectionPath],
            DatabaseName = configuration[AppDatabaseNamePath],
        }, serviceKey: DatabaseSettings.AppSettings);
        
        registrator.Register<IMongoContext, MongoContext>(Reuse.Singleton);
        
        registrator.RegisterInstance<IPaymentGatewaySettings>(new PaymentGatewaySettings
        {
            Domain = configuration[PaymentGatewayDomainPath],
            EnvironmentUrl = configuration[PaymentGatewayEnvironmentUrlPath],
            ApiUrl = configuration[PaymentGatewayApiUrlPath],
        });

        return registrator;
    }
}
