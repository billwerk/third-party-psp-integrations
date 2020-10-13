using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IPaymentTransactionService : IServiceBase<PaymentTransactionBase>
    {
        PaymentTransactionBase SingleByExternalTransactionIdOrDefault(string externalTransactionId);
    }
}