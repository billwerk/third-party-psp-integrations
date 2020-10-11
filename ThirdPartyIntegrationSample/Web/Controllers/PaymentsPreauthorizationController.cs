using System.Text.Json;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Business;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsPreauthorizationController : ApiControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IPaymentServiceWrapper _paymentService;

        public PaymentsPreauthorizationController(ICheckoutService checkoutService, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor, IPaymentServiceWrapper paymentService) : base(paymentServiceMethodsExecutor)
        {
            _checkoutService = checkoutService;
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("api/checkout")]
        public ObjectResult Checkout([FromBody] JsonElement json)
        {
            return BuildResponseFromResult<CheckoutResult, string>(_checkoutService.Checkout(JsonSerializer.Serialize(json)));
        }
        
        [HttpPost]
        [Route("api/preauth")]
        public async Task<ObjectResult> Preauthorize([FromBody] ExternalPreauthRequestDTO dto)
        {
            return BuildResponse(await _paymentService.SendPreauth(dto));
        }
    }
}