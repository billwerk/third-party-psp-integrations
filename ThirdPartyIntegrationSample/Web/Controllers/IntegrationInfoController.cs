using System.Collections.Generic;
using System.Linq;
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
            var settings = new ExternalIntegrationInfoDTO
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
                },
                MerchantSettings = new List<MerchantSettingDescription>
                {
                    new MerchantSettingDescription
                    {
                        DisplayName = "MerchantId",
                        KeyName = "MerchantId",
                        PlaceHolder = "MerchantId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "AccountId",
                        KeyName = "AccountId",
                        PlaceHolder = "AccountId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "PortalId",
                        KeyName = "PortalId",
                        PlaceHolder = "PortalId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "Key",
                        KeyName = "Key",
                        PlaceHolder = "Key",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "PortalIdRecurring",
                        KeyName = "PortalIdRecurring",
                        PlaceHolder = "PortalIdRecurring",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "KeyRecurring",
                        KeyName = "KeyRecurring",
                        PlaceHolder = "KeyRecurring",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "HashCalculationMethod",
                        KeyName = "HashCalculationMethod",
                        PlaceHolder = "HashCalculationMethod",
                        Required = true,
                        PredefinedValues = new SortedDictionary<string, string>()
                    }
                }
            };

            PopulatePredefinedHashAlgorithms(settings);

            return settings;
        }

        private static void PopulatePredefinedHashAlgorithms(ExternalIntegrationInfoDTO settings)
        {
            var hashCalculationMethodSetting = settings.MerchantSettings.First(s => s.KeyName == "HashCalculationMethod");
            hashCalculationMethodSetting.PredefinedValues.Add("0", "SHA384");
            hashCalculationMethodSetting.PredefinedValues.Add("1", "MD5");
        }
    }
}