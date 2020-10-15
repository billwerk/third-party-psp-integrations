using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment/{id}/refund")]
    public class PaymentsRefundsController : ApiControllerBase
    {
        private readonly IPaymentServiceWrapper _paymentService;

        [HttpGet]
        public async Task<ObjectResult> Get(string id)
        {
            return BuildResponse(await _paymentService.FetchRefund(id));
        }
        
        [HttpPost]
        public async Task<ObjectResult> Refund(ExternalRefundRequestDTO dto)
        {
            return BuildResponse(await _paymentService.SendRefund(dto));
        }

        public PaymentsRefundsController(IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor,
            IPaymentServiceWrapper paymentService) : base(paymentServiceMethodsExecutor)
        {
            _paymentService = paymentService;
        }
    }
}