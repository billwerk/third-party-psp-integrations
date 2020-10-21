namespace Business.Models
{
    public class ExternalPaymentWebhookDTO
    {
        public ExternalPaymentWebhookDTO(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; set; }
    }
}