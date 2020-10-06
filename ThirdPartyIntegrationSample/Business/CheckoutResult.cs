using Core.Rest;

namespace Business
{
    public class CheckoutResult : ResultBase<string>
    {
        public CheckoutResult(string data)
        {
            Data = data;
        }

        public CheckoutResult(string code, string message) 
            : base(code, message)
        {
        }
    }
}