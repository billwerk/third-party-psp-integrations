using Billwerk.Payment.SDK.DTO.IntegrationInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class IntegrationInfoController : ApiControllerBase
    {
        [HttpGet]
        [Route("api/integrationInfo")]
        public ExternalIntegrationInfoDTO Get()
        {
            return new ExternalIntegrationInfoDTO();
        }
    }
}