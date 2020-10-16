using Persistence.Models;
using Persistence.Mongo;

namespace Persistence.Interfaces
{
    public interface IPaymentTransactionService : IServiceBase<PaymentTransactionBase>
    {
        PaymentTransactionBase SingleByExternalTransactionIdOrDefault(string externalTransactionId);
        
        SinglePspTransaction SinglePspTransactionByProviderTransactionId(string providerTransactionId);
        
        PaymentTransaction SingleByPreauthTransactionId(ObjectId<PreauthTransaction> preauthTransactionId);

        bool UpdateTransactionSeqNumber(PaymentTransactionBase paymentTransaction, int sequenceNumber);
    }
}