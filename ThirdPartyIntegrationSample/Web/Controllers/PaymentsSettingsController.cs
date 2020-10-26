using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Enums;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class PaymentsSettingsController : ApiControllerBase
    {
        //TODO move to api/payone
        
        private readonly IExternalSettingsValidatorWrapper _externalSettingsValidator;

        public PaymentsSettingsController(IExternalSettingsValidatorWrapper externalSettingsValidator, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) 
            : base(paymentServiceMethodsExecutor)
        {
            _externalSettingsValidator = externalSettingsValidator;
        }

        [HttpPost]
        [Route("api/validateSettings")]
        public ObjectResult Validate(ExternalIntegrationValidateSettingsRequestDTO dto)
        {
            var result = _externalSettingsValidator.Validate(PaymentServiceProvider.PayOne, dto);
            if (result.Errors != null && result.Errors.Count > 0)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
