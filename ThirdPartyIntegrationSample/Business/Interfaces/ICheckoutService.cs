namespace Business.Interfaces
{
    
    public interface ICheckoutService
    {
        CheckoutResult Checkout(string json);
    }
    
}