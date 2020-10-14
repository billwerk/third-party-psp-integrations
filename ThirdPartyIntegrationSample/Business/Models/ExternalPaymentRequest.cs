using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;

namespace Business.Models
{
    public class ExternalPaymentRequest
    {
        public ExternalPaymentRequestDTO PaymentRequestDto { get; set; }

        public ExternalPreauthRequestDTO PreauthRequestDto { get; set; }

        public PaymentBearerDTO BearerDto { get; set; }

        public string PspTransactionId { get; set; }
    }
}