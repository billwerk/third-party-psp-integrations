using Billwerk.Payment.SDK.DTO.Preauth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsPreauthorizationController : ApiControllerBase
    {
        [HttpPost]
        [Route("api/checkout")]
        public ExternalPreauthTransactionDTO Preauthorize(ExternalPreauthRequestDTO dto)
        {
            return new ExternalPreauthTransactionDTO();
        }
    }
}