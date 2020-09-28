using Billwerk.Payment.SDK.DTO.Refund;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/payment/{id}/refund")]
    public class PaymentsRefundsController : ApiControllerBase
    {
        [HttpGet]
        public ExternalRefundTransactionDTO Get(string id)
        {
            return new ExternalRefundTransactionDTO();
        }
        
        [HttpPost]
        public ExternalRefundTransactionDTO Refund(ExternalRefundRequestDTO dto)
        {
            return new ExternalRefundTransactionDTO();
        }
    }
}