namespace Business.PayOne.Model.Requests
{
    public class RefundRequest : RequestBase
    {
        public RefundRequest(bool initialPayment, PSPSettingsPayOne settings)
            : base(initialPayment, settings)
        {
        }

        public string TxId { get; set; }
        /// <summary>
        /// Sequence number for this transaction within the payment process (1..n) e.g. authorization: 0, refund: 1 e.g. preauthorization: 0, capture: 1, refund: 2
        /// </summary>
        public string SequenceNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string NarrativeText { get; set; }
        public string TransactionParam { get; set; }
    }
}