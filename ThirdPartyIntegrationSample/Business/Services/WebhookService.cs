using System.Net;
using System.Net.Http;
using Business.Interfaces;
using Business.Models;
using Core.Interfaces;
using Core.Rest;
using Hangfire;
using Newtonsoft.Json;

namespace Business.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IRestClient _restClient;

        public WebhookService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 240 })]
        public void Send(string dispatchUrl, string transactionId)
        {
            var webhook = new ExternalPaymentWebhookDTO(transactionId);
            var restResult =
                _restClient.ExecuteAsync(dispatchUrl, HttpMethod.Post, JsonConvert.SerializeObject(webhook), HttpContentType.JsonStringContent).Result;

            if (!restResult.IsSuccessStatusCode || restResult.StatusCode != HttpStatusCode.OK)
            {
                //Todo: Exception to retry
                throw new WebException();
            }
        }
    }
}