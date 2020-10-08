using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsSettingsController : ApiControllerBase
    {
        private readonly IExternalSettingsValidator _externalSettingsValidator;

        public PaymentsSettingsController(IExternalSettingsValidator externalSettingsValidator, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) 
            : base(paymentServiceMethodsExecutor)
        {
            _externalSettingsValidator = externalSettingsValidator;
        }

        [HttpPost]
        [Route("api/validateSettings")]
        public ObjectResult Validate(ExternalIntegrationValidateSettingsRequestDTO dto)
        {
            var result = _externalSettingsValidator.Validate(dto);
            if (result.Errors != null && result.Errors.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
