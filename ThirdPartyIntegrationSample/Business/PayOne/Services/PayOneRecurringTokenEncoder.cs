using Business.Interfaces;
using Business.PayOne.Model;
using Business.Services;

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