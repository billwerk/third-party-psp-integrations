﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Billwerk.Payment.PayOne.Helpers;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Billwerk.Payment.SDK.Interfaces;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOneExternalSettingsValidator : IExternalSettingsValidator
    {
        private static ReadOnlyCollection<string> MerchantSettingsKeys =>
            new ReadOnlyCollection<string>(new[]
            {
                PayOneConstants.KeyKey,
                PayOneConstants.AccountIdKey,
                PayOneConstants.KeyRecurringKey,
                PayOneConstants.MerchantIdKey,
                PayOneConstants.PortalIdKey,
                PayOneConstants.PortalIdRecurringKey
            });

        public List<ExternalIntegrationErrorDTO> ValidateIntegrationInfo(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            return null;
        }

        public List<ExternalIntegrationErrorDTO> ValidateMerchantSettings(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;

            foreach (var predefined in MerchantSettingsKeys)
            {
                if (externalIntegrationSettings.MerchantSettings.Select(ms => ms.KeyName).Contains(predefined) == false)
                {
                    AddError(ref errors, $"The '{predefined}' setting is missing.");
                }
            }

            foreach (var merchantSetting in externalIntegrationSettings.MerchantSettings
                .Where(merchantSetting => MerchantSettingsKeys.Contains(merchantSetting.KeyName))
                .Where(merchantSetting => string.IsNullOrWhiteSpace(merchantSetting.KeyValue)))
            {
                AddError(ref errors, $"The '{merchantSetting.KeyName}' setting is empty.");
            }

            return errors;
        }
        
        private static void AddError(ref List<ExternalIntegrationErrorDTO> errors, string errorMessage)
        {
            errors ??= new List<ExternalIntegrationErrorDTO>();

            errors.Add(new ExternalIntegrationErrorDTO
            {
                ErrorMessage = errorMessage
            });
        }        
    }
}