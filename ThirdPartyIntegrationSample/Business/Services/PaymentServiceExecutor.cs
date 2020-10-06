using Business.Interfaces;

namespace Business.Services
{
    public class PaymentServiceExecutor : IPaymentServiceExecutor
    {
        private const int DelayInSeconds = 30;
        public void ExecuteAsynchronously()
        {
            //BackgroundJob.Schedule(act.Invoke(), TimeSpan.FromSeconds(DelayInSeconds));
        }
    }
}