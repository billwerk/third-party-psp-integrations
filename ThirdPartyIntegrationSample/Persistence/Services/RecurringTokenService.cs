using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class RecurringTokenService 
        : ServiceBase<RecurringToken>, IRecurringTokenService
    {
        public RecurringTokenService(IMongoContext db) 
            : base(db)
        {
        }
    }
}