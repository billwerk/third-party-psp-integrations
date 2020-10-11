using Business.Models;

namespace Business.Interfaces
{
    public interface IInitialTokenDecoder
    {
        public PayOnePspBearer Decode(string data);
    }
}