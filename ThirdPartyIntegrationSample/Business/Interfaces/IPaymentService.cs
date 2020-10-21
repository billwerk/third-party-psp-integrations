using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Models;
using Persistence.Models;

namespace Business.Interfaces
{
    public interface IPaymentService
    {
        Task<ExternalPaymentResponse> SendPayment(ExternalPaymentRequest paymentRequest);
        Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto, PaymentTransactionBase targetTransaction);
        Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto);
        Task<ExternalPaymentCancellationDTO> SendCancellation(ExternalPreauthRequestDTO dto, PaymentTransactionBase targetTransaction);
        Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId);
        Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId);
        Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId);
    }
}