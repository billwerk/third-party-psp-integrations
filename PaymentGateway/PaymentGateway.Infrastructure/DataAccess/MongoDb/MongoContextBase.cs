using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using PaymentGateway.Infrastructure.DataAccess.Database;
using PaymentGateway.Infrastructure.DataAccess.MongoDb.Serialization;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb;

public abstract class MongoContextBase : IMongoContext
{
    protected IMongoClient Client { get; }
    
    public IMongoDatabase Database { get; }
        
    public IDatabaseSettings Settings { get; }

    protected MongoContextBase(IDatabaseSettings settings)
    {
        Client = new MongoClient(settings.ConnectionString);
        ConventionRegistry.Register("Ignore extra elements", new ConventionPack
        {
            new IgnoreExtraElementsConvention(true),
        }, _ => true);

        Database = CreateSession(settings);
        Settings = settings;
    }
    
    private IMongoDatabase CreateSession(IDatabaseSettings settings)
    {
        var database = Client.GetDatabase(settings.DatabaseName);

        if (database is null)
            throw new InvalidOperationException("Failed to create database.");

        return database;
    }

    static MongoContextBase() => RegisterBsonSerializers.EnsureRegistered();

    public IMongoCollection<T> GetCollection<T>() => GetCollection<T>(typeof(T).Name);

    public IMongoCollection<T> GetCollection<T>(string name) => Database.GetCollection<T>(name);
}
