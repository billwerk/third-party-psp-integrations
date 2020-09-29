using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
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
            return new ExternalIntegrationInfoDTO
            {
                SupportRefunds = true,
                UsePaymentDataConfirmationFlow = false,
                UsePaymentDataConfirmationFlowForPreauth = false,
                UsesScheduledPayment = false,
                SupportMultipleTransactionOverpayments = true,
                RequiresReturnUrl = true,
                CreditCardMethodInfo = new ExternalIntegrationMethodInfoDTO
                {
                    UseCapturePreauth = true,
                    UseCancelPreauth = true,
                    DefaultPreauthAmount = decimal.One
                },
                DebitMethodInfo = new ExternalIntegrationDirectDebitMethodInfoDTO
                {
                    UseCapturePreauth = true,
                    UseCancelPreauth = true,
                    DefaultPreauthAmount = decimal.One,
                    SupportExternalMandateReference = true,
                    UnconfirmedLedgerForRefund = false,
                    UnconfirmedLedger = false,
                    SupportBackendPayments = true,
                    SupportedExternalMandateReferenceSize = 35,
                    SupportLowerCaseMandateReference = true
                },
                OnAccountMethodInfo = new ExternalIntegrationMethodInfoDTO
                {
                    UseCapturePreauth = false,
                    UseCancelPreauth = false,
                    DefaultPreauthAmount = decimal.One
                }
            };
        }
    }
}