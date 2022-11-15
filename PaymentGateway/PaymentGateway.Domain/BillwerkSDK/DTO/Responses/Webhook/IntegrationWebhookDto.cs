using PaymentGateway.Domain.BillwerkSDK.Enums;

namespace Billwerk.Payment.SDK.DTO.Responses.Webhook
{
    /// <summary>
    /// Model of webhook which must be passed to billwerk as payment notification.
    /// </summary>
    public class IntegrationWebhookDto
    {
        /// <summary>
        /// For deserializer only.
        /// </summary>
        public IntegrationWebhookDto()
        {
        }

        public IntegrationWebhookDto(string transactionId, WebhookType webhookType)
        {
            TransactionId = transactionId;
            Type = webhookType;
        }
        public IntegrationWebhookDto(string transactionId, string agreementId)
        {
            AgreementId = agreementId;
            TransactionId = transactionId;
            Type = WebhookType.Agreement;
        }

        /// <summary>
        /// Id of transaction in billwerk system for which event occured.
        /// </summary>
        public string TransactionId { get; set; }
        
        public string AgreementId { get; set;}
        
        /// <summary>
        /// Type of payment notification.
        /// </summary>
        public WebhookType Type { get; set;}
    }
}
