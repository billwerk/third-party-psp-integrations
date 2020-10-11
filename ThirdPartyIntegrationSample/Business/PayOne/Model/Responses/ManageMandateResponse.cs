namespace Business.PayOne.Model.Responses
{
    public class ManageMandateResponse : ResponseBase
    {
        public ManageMandateResponse(string response) 
            : base(response)
        {
        }

        public string Mandate_Identification { get; set; }

        public string Mandate_Status { get; set; }
        public string Mandate_Text { get; set; }
        public string Creditor_Identifier { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankCountry { get; set; }
    }
}