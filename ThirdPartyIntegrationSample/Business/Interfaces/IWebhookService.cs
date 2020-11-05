using Business.Enums;

using System.Threading.Tasks;
using Hangfire;

namespace Business.Interfaces
{
    public interface IWebhookService
    {
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 120, 240 })]
        public Task Send(string dispatchUrl,PaymentServiceProvider provider,  string transactionId);
    }
}