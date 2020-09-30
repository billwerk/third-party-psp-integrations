using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
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
        public ExternalIntegrationValidateSettingsDTO Validate(ExternalIntegrationValidateSettingsRequestDTO dto)
        {
            return new ExternalIntegrationValidateSettingsDTO();
        }
    }
}