using Core.Helpers;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Helpers;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class PaymentTransactionService : ServiceBase<PaymentTransactionBase>, IPaymentTransactionService
    {
        public PaymentTransactionService(IMongoContext db) : base(db)
        {
        }

        public PaymentTransactionBase SingleByExternalTransactionIdOrDefault(string externalTransactionId)
        {
            var results =
                Db.Find<PaymentTransactionBase>(Query<PaymentTransactionBase>.EQ(p => p.ExternalTransactionId,
                    externalTransactionId));
            return results.SingleOrDefault();
        }

        public SinglePspTransaction SinglePspTransactionByProviderTransactionId(string providerTransactionId)
        {
            var result = Db.Find<PaymentTransactionBase>(Query<PaymentTransactionBase>.EQ(p => p.PspTransactionId,
                providerTransactionId));

            return SinglePspTransaction.GetFromIFindFluent(result);
        }

        public PaymentTransaction SingleByPreauthTransactionId(ObjectId<PreauthTransaction> preauthTransactionId)
        {
            var result = Db.Find<PaymentTransactionBase>(Query.And(
                Query<PaymentTransaction>.EQ(p => p.PreauthTransactionId, preauthTransactionId.Untyped),
                MongoQuery.TypeEq(nameof(PaymentTransaction)))).SingleOrDefault();

            return result as PaymentTransaction;
        }

        public bool UpdateTransactionSeqNumber(PaymentTransactionBase paymentTransaction, int sequenceNumber)
        {
            var count = Db.UpdateSafe<PaymentTransactionBase>(
                Query<PaymentTransactionBase>.EQ(p => p.Id, paymentTransaction.Id),
                Update<PaymentTransactionBase>.Set(p => p.SequenceNumber, sequenceNumber));

            if (count <= 0) 
                return false;

            paymentTransaction.SequenceNumber = sequenceNumber;

            return true;
        }
    }
}