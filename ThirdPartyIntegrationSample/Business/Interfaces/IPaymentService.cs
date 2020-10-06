using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;

namespace Business.Interfaces
{
    public interface IPaymentService
    {
        CheckoutResult Checkout(string json);
        Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequestDTO dto);
        Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto);
        Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto);
        Task<ExternalPaymentCancellationDTO> SendCancellation(string transactionId);
        Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId);
        Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId);
        Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId);
    }
}