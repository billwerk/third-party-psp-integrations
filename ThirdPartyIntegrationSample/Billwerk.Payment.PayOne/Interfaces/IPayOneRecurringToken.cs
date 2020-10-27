using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.Interfaces;

namespace Billwerk.Payment.PayOne.Interfaces
{
    //TODO do we need an interface?
    public interface IPayOneRecurringToken: IRecurringToken
    {
        string UserId { get; set; }
        
        PaymentBearerDTO PaymentBearer { get; set; }
        
        PayOnePspBearer PspBearer { get; set; }
    }
}