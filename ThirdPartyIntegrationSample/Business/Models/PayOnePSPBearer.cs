using System;

namespace Business.Models
{
    public class PayOnePspBearer
    {
        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string Holder { get; set; }

        public string Country { get; set; }

        public string PseudoCardPan { get; set; }
        
        public string TruncatedCardPan { get; set; }

        public string CardType { get; set; }

        public string Code { get; set; }

        public string Account { get; set; }

        public string Iban { get; set; }

        public string Bic { get; set; }
        
        
        public string MandateReference { get; set; }
        
        
        public DateTime? MandateSignatureDate { get; set; }
        
        
        public string MandateText { get; set; }
        
        
        public string CreditorId { get; set; }
    }
}