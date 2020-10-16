namespace Persistence.Models
{
    public class PreauthPaymentPair
    {
        public PreauthTransaction Preauth { get; }
        
        public PaymentTransaction Payment { get; }

        public PreauthPaymentPair(PreauthTransaction preauth, PaymentTransaction payment)
        {
            Preauth = preauth;
            Payment = payment;
        }
    }
}