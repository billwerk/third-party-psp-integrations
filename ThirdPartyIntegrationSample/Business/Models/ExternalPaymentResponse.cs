using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;

namespace Business.Models
{
    public class ExternalPaymentResponse
    {
        public ExternalPaymentResponse(ExternalPaymentTransactionDTO paymentDto)
        {
            PaymentDto = paymentDto;
        }

        public ExternalPaymentTransactionDTO PaymentDto { get; set; }

        public string RecurringToken { get; set; }
    }
}