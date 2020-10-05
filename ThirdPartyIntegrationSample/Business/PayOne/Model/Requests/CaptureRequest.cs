namespace Business.PayOne.Model.Requests
{
    public class CaptureRequest : RequestBase
    {
        public CaptureRequest(bool initialPayment, PayOnePSPSettings settings) 
            : base(initialPayment, settings)
        {
        }

        public string TxId { get; set; }
        public string SequenceNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string NarrativeText { get; set; }
        public string TransactionParam { get; set; }
        public string DueTime { get; set; }
    }
}