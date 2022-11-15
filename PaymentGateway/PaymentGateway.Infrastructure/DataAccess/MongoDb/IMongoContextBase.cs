using MongoDB.Driver;
using PaymentGateway.Infrastructure.DataAccess.Database;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb;

public interface IMongoContextBase
{
    IMongoDatabase Database { get; }
    
    IDatabaseSettings Settings { get; }
    
    public IMongoCollection<T> GetCollection<T>();

    public IMongoCollection<T> GetCollection<T>(string name);
}
