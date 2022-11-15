namespace PaymentGateway.Infrastructure.DataAccess.Database;

public interface IDatabaseSettings
{
    public string ConnectionString { get; init; }
    
    public string DatabaseName { get; init; }
}