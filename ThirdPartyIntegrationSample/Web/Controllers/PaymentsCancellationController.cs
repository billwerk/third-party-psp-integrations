using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsCancellationController : ApiControllerBase
    {
        private readonly IPaymentServiceMethodsExecutor _paymentServiceMethodsExecutor;

        public PaymentsCancellationController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) 
            : base(paymentServiceMethodsExecutor)
        {
            _paymentServiceMethodsExecutor = paymentServiceMethodsExecutor;
        }

        [HttpPost]
        [Route("api/payment/{id}/cancel")]
        public ExternalPaymentCancellationDTO Cancel(string id)
        {
            ExecutePaymentServiceMethodAsynchronously(x => x.SendCancellation(id));
            
            _paymentServiceMethodsExecutor.ExecuteAsynchronously(x => x.SendCancellation(id));
            
            return new ExternalPaymentCancellationDTO();
        }
    }
}