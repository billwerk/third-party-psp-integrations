using Billwerk.Payment.SDK.Interfaces.Models;

namespace Core.Rest
{
    public class PspResult: RestResult<string>, IPspResponse
    {
        public string RawData
        {
            get => Data;
            set => Data = value;
        }
    }
}