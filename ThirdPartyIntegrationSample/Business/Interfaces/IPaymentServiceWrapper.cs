using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Business.Interfaces
{
    public interface IPaymentServiceWrapper
    { 
        ObjectResult HandleWebhookAsync(string requestString);
        Task<ExternalPaymentTransactionDTO> SendPayment(PaymentServiceProvider provider, ExternalPaymentRequestDTO paymentDto);
        Task<ExternalRefundTransactionDTO> SendRefund(PaymentServiceProvider provider, string transactionId, ExternalRefundRequestDTO dto);
        Task<ExternalPreauthTransactionDTO> SendPreauth(PaymentServiceProvider provider, ExternalPreauthRequestDTO dto);
        Task<ExternalPaymentTransactionDTO> FetchPayment(PaymentServiceProvider provider, string transactionId);
        Task<ExternalRefundTransactionDTO> FetchRefund(PaymentServiceProvider provider, string transactionId);
        Task<ExternalPreauthTransactionDTO> FetchPreauth(PaymentServiceProvider provider, string transactionId);
        public Task<ExternalPaymentCancellationDTO> SendCancellation(PaymentServiceProvider provider, string transactionId);
    }
}