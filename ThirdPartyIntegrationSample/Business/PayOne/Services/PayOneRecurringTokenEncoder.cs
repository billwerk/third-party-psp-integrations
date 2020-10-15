using Business.Interfaces;
using Business.Services;
using Persistence.Models;

namespace Business.PayOne.Services
{
    public class PayOneRecurringTokenEncoder : RecurringTokenEncoderBase<RecurringToken>
    {
        public PayOneRecurringTokenEncoder(ITetheredPaymentInformationEncoder tetheredPaymentInformationEncoder) 
            : base(tetheredPaymentInformationEncoder)
        {
        }
    }
}