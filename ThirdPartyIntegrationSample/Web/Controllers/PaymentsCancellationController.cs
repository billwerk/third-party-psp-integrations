using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsCancellationController : ApiControllerBase
    {
        [HttpPost]
        [Route("api/payment/{id}/cancel")]
        public ExternalPaymentCancellationDTO Cancel(string id)
        {
            return new ExternalPaymentCancellationDTO();
        }
    }
}