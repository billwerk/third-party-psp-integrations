namespace Business.PayOne.Model.Requests
{
    public class Capture : RequestBase
    {
        public Capture(bool initialPayment, PayOnePSPSettings settings) 
            : base(initialPayment, settings)
        {
        }

        public string TxId { get; set; }

        //Sequence number for this transaction within the payment process(1..n)
        public string SequenceNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Narrative_Text { get; set; }
        public string Transaction_Param { get; set; }
        public string Due_Time { get; set; }
    }
}