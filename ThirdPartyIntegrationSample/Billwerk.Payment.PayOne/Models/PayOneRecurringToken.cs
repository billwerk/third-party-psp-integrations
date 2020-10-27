using Billwerk.Payment.PayOne.Interfaces;
using Billwerk.Payment.SDK.DTO;

namespace Billwerk.Payment.PayOne.Models
{
    public class PayOneRecurringToken: IPayOneRecurringToken
    {
        public string UserId { get; set; }
        public PaymentBearerDTO PaymentBearer { get; set; }
        public PayOnePspBearer PspBearer { get; set; }
        public string Token { get; }
    }
}