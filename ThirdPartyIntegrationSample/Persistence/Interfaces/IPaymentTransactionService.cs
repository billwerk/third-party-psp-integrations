using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IPaymentTransactionService : IServiceBase<PaymentTransaction>
    {
        PaymentTransaction SingleByExternalTransactionIdOrDefault(string externalTransactionId);
    }
}