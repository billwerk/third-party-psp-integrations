namespace Business.PayOne.Model.Requests
{
    public class Authorization : RequestBase
    {
        public Authorization(bool initialPayment, PayOnePSPSettings settings) 
            : base(initialPayment, settings)
        {
            AId = settings.AccountId;
        }

        public string AId { get; set; }
        public string ClearingType { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }

        // Optional
        public string Param { get; set; }
        public string Narrative_Text { get; set; }
        public string Transaction_Param { get; set; }
        public Customer Customer { get; set; }
        public PaymentBearer PaymentBearer { get; set; }

        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }

        public string ECommerceMode { get; set; }
        public string Due_Time { get; set; }
    }
}