namespace PaymentGateway.Infrastructure.DataAccess.Database;

public class DatabaseSettings : IDatabaseSettings
{
    public const string AppSettings = nameof(AppSettings);

    public string ConnectionString { get; init; }

    public string DatabaseName { get; init; }
}
