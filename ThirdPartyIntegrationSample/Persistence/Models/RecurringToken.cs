using Billwerk.Payment.SDK.DTO;

namespace Persistence.Models
{
    public class RecurringToken : DbObject
    {
        public string UserId { get; set; }
        
        public PaymentBearerDTO PaymentBearer { get; set; }
        
        public PspBearer PspBearer { get; set; }
    }
}