using Billwerk.Payment.PayOne.Interfaces;
using Business.Models;
using Newtonsoft.Json;
using Persistence.Models;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOneInitialTokenDecoder : IPayOneInitialTokenDecoder
    {
        public PspBearer Decode(string data)
        {
            return JsonConvert.DeserializeObject<PayOnePspBearer>(data);
        }
    }
}