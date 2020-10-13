using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class PaymentTransactionService 
        : ServiceBase<PaymentTransactionBase>, IPaymentTransactionService
    {
        public PaymentTransactionService(IMongoContext db) 
            : base(db)
        {
        }

        public PaymentTransactionBase SingleByExternalTransactionIdOrDefault(string externalTransactionId)
        {
            var results = Db.Find<PaymentTransactionBase>(Query<PaymentTransactionBase>.EQ(p => p.ExternalTransactionId, externalTransactionId));
            return results.SingleOrDefault();
        }
    }
}