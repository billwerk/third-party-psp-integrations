using System.Text.Json;
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

        public PaymentsPreauthorizationController(ICheckoutService checkoutService, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) : base(paymentServiceMethodsExecutor)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        [Route("api/checkout")]
        public ObjectResult Checkout([FromBody] JsonElement json)
        {
            return BuildResponseFromResult<CheckoutResult, string>(_checkoutService.Checkout(JsonSerializer.Serialize(json)));
        }
        
        [HttpPost]
        [Route("api/preauth")]
        public ExternalPreauthTransactionDTO Preauthorize(ExternalPreauthRequestDTO dto)
        {
            return new ExternalPreauthTransactionDTO();
        }
    }
}