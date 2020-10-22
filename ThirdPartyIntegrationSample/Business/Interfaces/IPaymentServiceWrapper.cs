using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Microsoft.AspNetCore.Mvc;

namespace Business.Interfaces
{
    public interface IPaymentServiceWrapper
    { 
        ObjectResult HandleWebhookAsync(string requestString);
        Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO paymentDto);
        Task<ExternalRefundTransactionDTO> SendRefund(string transactionId, ExternalRefundRequestDTO dto);
        Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto);
        Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId);
        Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId);
        Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId);
        public Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId);
    }
}