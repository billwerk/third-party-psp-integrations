using Business.Interfaces;

namespace Business.PayOne.Services
{
    public abstract class PaymentServiceBase
    {
        private readonly ITetheredPaymentInformationEncoder _paymentInformationEncoder;

        protected PaymentServiceBase(ITetheredPaymentInformationEncoder paymentInformationEncoder)
        {
            _paymentInformationEncoder = paymentInformationEncoder;
        }

        public CheckoutResult Checkout(string json)
        {
            return new CheckoutResult(_paymentInformationEncoder.Encrypt(json));
        }
    }
}