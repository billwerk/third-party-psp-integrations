using System.Collections.Generic;
using Core.Helpers;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Persistence.Helpers;
using Persistence.Interfaces;
using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Services
{
    public class PaymentTransactionService : ServiceBase<Transaction>, IPaymentTransactionService
    {
        public PaymentTransactionService(IMongoContext db) : base(db)
        {
        }

        public Transaction SingleByExternalTransactionIdOrDefault(string externalTransactionId)
        {
            var results =
                Db.Find<Transaction>(Query<Transaction>.EQ(p => p.ExternalTransactionId,
                    externalTransactionId));
            return results.SingleOrDefault();
        }

        public SinglePspTransaction SinglePspTransactionByProviderTransactionId(string providerTransactionId)
        {
            var result = Db.Find<Transaction>(Query<Transaction>.EQ(p => p.PspTransactionId,
                providerTransactionId));

            return SinglePspTransaction.GetFromIFindFluent(result);
        }

        public PaymentTransaction SingleByPreauthTransactionId(ObjectId<PreauthTransaction> preauthTransactionId)
        {
            var result = Db.Find<Transaction>(Query.And(
                Query<PaymentTransaction>.EQ(p => p.PreauthTransactionId, preauthTransactionId.Untyped),
                MongoQuery.TypeEq(nameof(PaymentTransaction)))).SingleOrDefault();

            return result as PaymentTransaction;
        }

        public bool UpdateTransactionSeqNumber(Transaction transaction, int sequenceNumber)
        {
            var count = Db.UpdateSafe<Transaction>(
                Query<Transaction>.EQ(p => p.Id, transaction.Id),
                Update<Transaction>.Set(p => p.SequenceNumber, sequenceNumber));

            if (count <= 0) 
                return false;

            transaction.SequenceNumber = sequenceNumber;

            return true;
        }
    }
}