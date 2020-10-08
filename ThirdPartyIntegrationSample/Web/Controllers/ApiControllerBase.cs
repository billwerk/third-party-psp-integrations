using System;
using System.Linq.Expressions;
using Business.Interfaces;
using Core.Rest;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        private readonly IPaymentServiceMethodsExecutor _paymentServiceMethodsExecutor;

        protected ApiControllerBase(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor)
        {
            _paymentServiceMethodsExecutor = paymentServiceMethodsExecutor;
        }

        protected ObjectResult ExecutePaymentServiceMethodAsynchronously(Expression<Action<IPaymentService>> methodCall)
        {
            _paymentServiceMethodsExecutor.ExecuteAsynchronously(methodCall);

            return Ok("");
        }

        protected ObjectResult BuildResponseFromResult<TResult, TData>(TResult result) where TResult : ResultBase<TData>
        {
            if (result.HasError)
            {
                return BadRequest(result.Error);
            }
            else
            {
                return Ok(result.Data);
            }
        }
    }
}