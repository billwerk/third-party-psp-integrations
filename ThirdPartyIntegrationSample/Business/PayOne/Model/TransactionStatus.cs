namespace Business.PayOne.Model
{
    public class TransactionStatus : ModelBase
    {
        private readonly string _rawData;

        public TransactionStatus(string response)
        {
            _rawData = response;
            
            var codec = new NvCodec();
            codec.Decode(response);
            Decode(codec);
        }

        public string GetRawData()
        {
            return _rawData;
        }

        public string Key { get; set; }
        public string TxAction { get; set; }
        public string Mode { get; set; }
        public string PortalId { get; set; }
        public string AId { get; set; }
        public string ClearingType { get; set; }
        public string TxTime { get; set; }
        public string Currency { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public string Param { get; set; }
        public string TxId { get; set; }
        public string Reference { get; set; }
        public string FailedCause { get; set; }
        public string SequenceNumber { get; set; }
        //N..7,2 Total payment request (in largest currency unit! e.g. Euro); not set for enhancement reminder status information without paid amount
        public string Receivable { get; set; }
        public string Balance { get; set; }
    }
}