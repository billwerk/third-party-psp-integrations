using Billwerk.Payment.SDK.Interfaces;
using Business.Interfaces;

namespace Business.Services
{
    
    public class CheckoutService : ICheckoutService
    {
        private readonly ITetheredPaymentInformationEncoder _paymentInformationEncoder;

        public CheckoutService(ITetheredPaymentInformationEncoder paymentInformationEncoder)
        {
            _paymentInformationEncoder = paymentInformationEncoder;
        }

        public CheckoutResult Checkout(string json)
        {
            return new CheckoutResult(_paymentInformationEncoder.Encrypt(json));
        }
    }
    
}