using System;
using System.Linq.Expressions;
using Billwerk.Payment.SDK.Interfaces;
using Business.Interfaces;
using Hangfire;

namespace Business.Services
{
    public class PaymentServiceMethodsExecutor : IPaymentServiceMethodsExecutor
    {
        private const int DelayInSeconds = 30;
        public void ExecuteAsynchronously(Expression<Action<IPspPaymentService>> methodCall)
        {
            var jobId = BackgroundJob.Schedule(methodCall, TimeSpan.FromSeconds(DelayInSeconds));
            
            //Todo: Send webhook
            BackgroundJob.ContinueJobWith(
                jobId,
                () => Console.WriteLine("Continuation!"));
        }

        public void ExecuteSynchronously()
        {
            throw new System.NotImplementedException();
        }
    }
}