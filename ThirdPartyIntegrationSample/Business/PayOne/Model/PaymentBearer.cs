namespace Business.PayOne.Model
{
    public abstract class PaymentBearer : ModelBase
    {
    }

    public class Invoice : PaymentBearer
    {
    }

    public class DirectDebit : PaymentBearer
    {
        public string BankCountry { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankAccountHolder { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string MandateIdentification { get; set; }
    }

    public class OnlineTransaction : PaymentBearer
    {
        public string OnlineTransferType { get; set; }
        public string BankCountry { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankGroupType { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }
    }

    public class EWallet : PaymentBearer
    {
        public string WalletType { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }
    }

    public class PseudoCreditCard : PaymentBearer
    {
        public string PseudoCardPan { get; set; }
        public string CardType { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
    }
}
