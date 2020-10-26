using System.Collections.Generic;
using Billwerk.Payment.PayOne.Helpers;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Billwerk.Payment.SDK.Interfaces;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOneExternalIntegrationInfoService : IExternalIntegrationInfoService
    {
        public ExternalIntegrationInfoDTO Create()
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
                },
                MerchantSettings = new List<MerchantSettingDescription>
                {
                    new MerchantSettingDescription
                    {
                        DisplayName = "MerchantId",
                        KeyName = PayOneConstants.MerchantIdKey,
                        PlaceHolder = "MerchantId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "AccountId",
                        KeyName = PayOneConstants.AccountIdKey,
                        PlaceHolder = "AccountId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "PortalId",
                        KeyName = PayOneConstants.PortalIdKey,
                        PlaceHolder = "PortalId",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "Key",
                        KeyName = PayOneConstants.KeyKey,
                        PlaceHolder = "Key",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "PortalIdRecurring",
                        KeyName = PayOneConstants.PortalIdRecurringKey,
                        PlaceHolder = "PortalIdRecurring",
                        Required = true
                    },
                    new MerchantSettingDescription
                    {
                        DisplayName = "KeyRecurring",
                        KeyName = PayOneConstants.KeyRecurringKey,
                        PlaceHolder = "KeyRecurring",
                        Required = true
                    }
                }
            };
        }
    }
}