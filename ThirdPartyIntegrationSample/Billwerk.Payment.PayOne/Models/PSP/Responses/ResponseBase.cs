using Billwerk.Payment.PayOne.Helpers;

namespace Billwerk.Payment.PayOne.Model.Responses
{
    public abstract class ResponseBase : ModelBase
    {
        private readonly string _rawData;

        protected ResponseBase(string response)
        {
            var codec = new NvCodec();
            
            _rawData = response;
            codec.Decode(response);
            Decode(codec);
        }

        public string GetRawData()
        {
            return _rawData;
        }

        public string Status { get; set; }
        public string TxId { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        
        public string RedirectUrl { get; set; }
    }
}