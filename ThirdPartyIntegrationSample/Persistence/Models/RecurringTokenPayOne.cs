using Billwerk.Payment.PayOne;
using Billwerk.Payment.PayOne.Interfaces;
using Billwerk.Payment.SDK.DTO;

namespace Persistence.Models
{
    public class RecurringTokenPayOne: RecurringToken, IPayOneRecurringToken
    {
        public string UserId { get; set; }
        public PaymentBearerDTO PaymentBearer { get; set; }
        public PayOnePspBearer PspBearer { get; set; }
    }
}