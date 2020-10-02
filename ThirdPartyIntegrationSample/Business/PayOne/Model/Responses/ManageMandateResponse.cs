namespace Business.PayOne.Model.Responses
{
    public class ManageMandateResponse : ResponseBase
    {
        public ManageMandateResponse(string response) 
            : base(response)
        {
        }

        public string MandateIdentification { get; set; }
        public string MandateStatus { get; set; }
        public string MandateText { get; set; }
        public string CreditorIdentifier { get; set; }
        public string Iban { get; set; }
        public string Bic { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankCountry { get; set; }
    }
}