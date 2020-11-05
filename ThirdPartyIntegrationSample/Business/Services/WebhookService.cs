using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Webhook;
using Billwerk.Payment.SDK.Enums;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Models;
using Business.Enums;
using Business.Interfaces;
using Hangfire;

namespace Business.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IPspClient _restClient;

        public WebhookService(IPspClient restClient)
        {
            _restClient = restClient;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 240 })]
        public async Task Send(string dispatchUrl, PaymentServiceProvider provider, string transactionId)
        {
            var webhook = new ExternalPaymentWebhookDTO(transactionId);
            var l = new PspLoggingContext
            {
                PspType = provider.ToString(),
                TransactionId = transactionId
            };
            var restResult = await _restClient.ExecuteRequestAsync(dispatchUrl, HttpMethod.Post, webhook, l, HttpContentType.JsonStringContent);

            if (!restResult.IsSuccessStatusCode || restResult.StatusCode != HttpStatusCode.Accepted)
            {
                //Todo: Exception to retry
                throw new WebException();
            }
        }
    }
}