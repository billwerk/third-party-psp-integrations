using Billwerk.Payment.SDK.DTO.IntegrationInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsSettingsController : ApiControllerBase
    {
        [HttpPost]
        [Route("api/validateSettings")]
        public ExternalIntegrationSettingsErrorDTO Validate(ExternalIntegrationValidateSettingsRequestDTO dto)
        {
            return new ExternalIntegrationSettingsErrorDTO();
        }
    }
}