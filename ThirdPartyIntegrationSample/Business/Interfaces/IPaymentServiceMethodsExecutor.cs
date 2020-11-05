using System;
using System.Linq.Expressions;
using Billwerk.Payment.SDK.Interfaces;

namespace Business.Interfaces
{
    public interface IPaymentServiceMethodsExecutor
    {
        public void ExecuteAsynchronously(Expression<Action<IPspPaymentService>> methodCall);
        public void ExecuteSynchronously();
    }
}