using System.Threading.Tasks;
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
        private readonly IPaymentServiceWrapper _paymentService;

        public PaymentsCancellationController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor, IPaymentServiceWrapper paymentServiceWrapper) 
            : base(paymentServiceMethodsExecutor)
        {
            _paymentService = paymentServiceWrapper;
        }

        [HttpPost]
        [Route("api/payment/{id}/cancel")]
        public async Task<ObjectResult> Cancel(string id)
        {
            return BuildResponse(await _paymentService.SendCancellation(id));
        }
    }
}