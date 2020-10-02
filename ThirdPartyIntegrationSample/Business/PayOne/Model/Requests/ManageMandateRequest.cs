namespace Business.PayOne.Model.Requests
{
    public class ManageMandateRequest : RequestBase
    {
        public ManageMandateRequest(bool initialPayment, PSPSettingsPayOne settings)
            : base(initialPayment, settings)
        {
            AId = settings.AccountId;
        }

        public string AId { get; set; }
        public string ClearingType { get; set; }
        public string MandateIdentification { get; set; }
        public string Currency { get; set; }
        public Customer Customer { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankCountry { get; set; }
    }
}