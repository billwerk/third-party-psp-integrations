using MongoDB.Bson;
using MongoDB.Driver;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Infrastructure.DataAccess.MongoDb;

namespace PaymentGateway.Infrastructure.Modules;

public class SettingsRepository : ISettingsRepository
{
    private readonly IMongoContext _mongoContext;

    public SettingsRepository(IMongoContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public IMerchantPspSettings GetById(NotEmptyString pspSettingsId) =>
        _mongoContext.GetCollection<PspSettings>().AsQueryable().Single(x => x.Id == ObjectId.Parse(pspSettingsId));

    public IMerchantPspSettings GetDefault() => _mongoContext.GetCollection<PspSettings>().AsQueryable().FirstOrDefault();
    
    public void SaveSettings(PspSettings pspSettings)
    {
        var currentSettings = GetDefault();
        if (currentSettings is not null)
        {
            var idSettingsFilter = Builders<PspSettings>.Filter.Eq(x => x.Id, (currentSettings as PspSettings)?.Id);
            _mongoContext.GetCollection<PspSettings>().ReplaceOne(idSettingsFilter, pspSettings);
        }
        else
            _mongoContext.GetCollection<PspSettings>().InsertOne(pspSettings);
    }
}
