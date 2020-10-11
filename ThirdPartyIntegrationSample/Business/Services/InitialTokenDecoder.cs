using Business.Interfaces;
using Business.Models;
using Newtonsoft.Json;


namespace Business.Services
{
    public class InitialTokenDecoder : IInitialTokenDecoder
    {
        public PayOnePspBearer Decode(string data)
        {
            return JsonConvert.DeserializeObject<PayOnePspBearer>(data);
        }
    }
}