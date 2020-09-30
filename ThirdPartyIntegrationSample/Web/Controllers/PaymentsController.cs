using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment/{id}")]
    public class PaymentsController : ApiControllerBase
    {
        [HttpGet]
        public ExternalPaymentTransactionDTO Get(string id)
        {
            return new ExternalPaymentTransactionDTO();
        }
        
        [HttpPost]
        public ExternalPaymentTransactionDTO Pay(ExternalPaymentRequestDTO dto)
        {
            return new ExternalPaymentTransactionDTO();
        }
    }
}