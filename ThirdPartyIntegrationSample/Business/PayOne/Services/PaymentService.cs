using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Interfaces;

namespace Business.PayOne.Services
{
    public class PaymentService : PaymentServiceBase, IPaymentService
    {
        public PaymentService(ITetheredPaymentInformationEncoder paymentInformationEncoder) 
            : base(paymentInformationEncoder)
        {
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
            throw new System.NotImplementedException();
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