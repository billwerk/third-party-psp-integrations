using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Interfaces
{
    public interface IPaymentTransactionService : IServiceBase<Transaction>
    {
        Transaction SingleByExternalTransactionIdOrDefault(string externalTransactionId);
        
        SinglePspTransaction SinglePspTransactionByProviderTransactionId(string providerTransactionId);
        
        PaymentTransaction SingleByPreauthTransactionId(ObjectId<PreauthTransaction> preauthTransactionId);

        bool UpdateTransactionSeqNumber(Transaction transaction, int sequenceNumber);
    }
}