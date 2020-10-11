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
    }
}