using Billwerk.Payment.PayOne;

namespace Billwerk.Payment.PayOne.Model.Requests
{
    public class Preauthorization : RequestBase
    {
        public Preauthorization(bool initialPayment, PayOnePSPSettings settings)
            : base(initialPayment, settings)
        {
            AId = settings.AccountId;
        }

        public string AId { get; set; }
        public string ClearingType { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }

        //optional
        public string Param { get; set; }
        public string Narrative_Text { get; set; }

        public Customer Customer { get; set; }
        public PaymentBearer PaymentBearer { get; set; }

        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }

        public string ECommerceMode { get; set; }
    }
}