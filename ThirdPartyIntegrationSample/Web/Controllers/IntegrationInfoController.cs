using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Enums;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    [ApiController]
    public class IntegrationInfoController : ApiControllerBase
    {
        //TODO move to api/payone/integrationInfo
        
        private readonly IExternalIntegrationInfoWrapper _externalIntegrationInfoWrapper;

        public IntegrationInfoController(IExternalIntegrationInfoWrapper externalIntegrationInfoWrapper, IPaymentServiceMethodsExecutor paymentServiceMethodsExecutor) : base(paymentServiceMethodsExecutor)
        {
            _externalIntegrationInfoWrapper = externalIntegrationInfoWrapper;
        }

        [HttpGet]
        [Route("api/integrationInfo")]
        public ExternalIntegrationInfoDTO Get()
        {
            return _externalIntegrationInfoWrapper.Create(PaymentServiceProvider.PayOne);
        }
    }
}