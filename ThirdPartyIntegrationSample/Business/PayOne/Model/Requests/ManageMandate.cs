namespace Business.PayOne.Model.Requests
{
    public class ManageMandate : RequestBase
    {
        public ManageMandate(bool initialPayment, PayOnePSPSettings settings) 
            : base(initialPayment, settings)
        {
            AId = settings.AccountId;
        }

        public string AId { get; set; }
        public string ClearingType { get; set; }
        public string Mandate_Identification { get; set; }
        public string Currency { get; set; }
        public Customer Customer { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankCountry { get; set; }
    }
}