using System;
using System.Linq.Expressions;

namespace Business.Interfaces
{
    public interface IPaymentServiceMethodsExecutor
    {
        public void ExecuteAsynchronously(Expression<Action<IPaymentService>> methodCall);
        public void ExecuteSynchronously();
    }
}