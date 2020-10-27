using Billwerk.Payment.PayOne.Interfaces;
using Billwerk.Payment.SDK.Interfaces.Models;
using Business.Models;
using Newtonsoft.Json;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOneInitialTokenDecoder : IPayOneInitialTokenDecoder
    {
        public IPspBearer Decode(string data)
        {
            return JsonConvert.DeserializeObject<PayOnePspBearer>(data);
        }
    }
}