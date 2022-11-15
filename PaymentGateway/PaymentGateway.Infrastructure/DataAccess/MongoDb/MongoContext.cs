using PaymentGateway.Infrastructure.DataAccess.Database;

namespace PaymentGateway.Infrastructure.DataAccess.MongoDb;

public class MongoContext : MongoContextBase
{
    public MongoContext(IEnumerable<KeyValuePair<string, Func<IDatabaseSettings>>> settings)
        : base(settings.Single(x => x.Key == DatabaseSettings.AppSettings).Value()) { }
}
