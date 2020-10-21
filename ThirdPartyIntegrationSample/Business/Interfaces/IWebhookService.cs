namespace Business.Interfaces
{
    public interface IWebhookService
    {
        public void Send(string dispatchUrl, string transactionId);
    }
}