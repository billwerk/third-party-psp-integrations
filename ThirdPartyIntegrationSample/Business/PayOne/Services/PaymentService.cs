using System;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Interfaces;
using Persistence.Mongo;

namespace Business.PayOne.Services
{
    public class PaymentService : PaymentServiceBase, IPaymentService
    {
        private readonly IMongoContext _mongoContext;
        
        public PaymentService(IMongoContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

        public Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId)
        {
            Console.WriteLine(_mongoContext.Provider);
            
            return new Task<ExternalPaymentCancellationDTO>(() => new ExternalPaymentCancellationDTO());
        }

        public Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId)
        {
            throw new System.NotImplementedException();
        }
    }
}