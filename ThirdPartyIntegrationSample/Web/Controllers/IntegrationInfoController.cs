using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class IntegrationInfoController : ApiControllerBase
    {
        private readonly IExternalIntegrationInfoFactory _externalIntegrationInfoFactory;

        public IntegrationInfoController(IExternalIntegrationInfoFactory externalIntegrationInfoFactory, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) : base(paymentServiceMethodsExecutor)
        {
            _externalIntegrationInfoFactory = externalIntegrationInfoFactory;
        }

        [HttpGet]
        [Route("api/integrationInfo")]
        public ExternalIntegrationInfoDTO Get()
        {
            return _externalIntegrationInfoFactory.Create();
        }
    }
}