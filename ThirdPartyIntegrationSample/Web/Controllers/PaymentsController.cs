using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Business.Enums;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment/{id?}")]
    public class PaymentsController : ApiControllerBase
    {
        //TODO move to api/payone
        
        private readonly IPaymentServiceWrapper _paymentService;

        public PaymentsController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor,
            IPaymentServiceWrapper paymentService) : base(paymentServiceMethodsExecutor)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ObjectResult> Get(string id)
        {
            return BuildResponse(await _paymentService.FetchPayment(PaymentServiceProvider.PayOne, id));
        }

        [HttpPost]
        public async Task<ObjectResult> Pay(ExternalPaymentRequestDTO dto)
        {
            return BuildResponse(await _paymentService.SendPayment(PaymentServiceProvider.PayOne, dto));
        }
    }
}