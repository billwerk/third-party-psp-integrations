using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class PaymentTransactionService 
        : ServiceBase<PaymentTransaction>, IPaymentTransactionService
    {
        public PaymentTransactionService(IMongoContext db) 
            : base(db)
        {
        }

        public PaymentTransaction SingleByExternalTransactionIdOrDefault(string externalTransactionId)
        {
            var results = Db.Find<PaymentTransaction>(Query<PaymentTransaction>.EQ(p => p.ExternalTransactionId, externalTransactionId));
            return results.SingleOrDefault();
        }
    }
}