using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Interfaces;
using Business.PayOne.Helpers;

namespace Business.PayOne.Services
{
    public class PayOneExternalSettingsValidator : ExternalSettingsValidatorBase, IExternalSettingsValidator
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

        public PayOneExternalSettingsValidator(IExternalIntegrationInfoFactory externalIntegrationInfoFactory) : base(
            externalIntegrationInfoFactory)
        {
        }

        public ExternalIntegrationValidateSettingsDTO Validate(
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var errors = ValidateDto(externalIntegrationSettings);
            var hasErrors = errors != null && errors.Count > 0;
            
            return new ExternalIntegrationValidateSettingsDTO
            {
                IsCreditCardValid = !hasErrors,
                IsDebitValid = !hasErrors,
                IsOnAccountValid = !hasErrors,
                Errors = errors
            };
        }

        private List<ExternalIntegrationErrorDTO> ValidateDto(
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var result = new List<ExternalIntegrationErrorDTO>();

            void Validate(List<ExternalIntegrationErrorDTO> errors,
                Func<ExternalIntegrationValidateSettingsRequestDTO, List<ExternalIntegrationErrorDTO>> validateFunc)
            {
                var validationErrors = validateFunc(externalIntegrationSettings);
                if (validationErrors != null)
                {
                    errors.AddRange(validationErrors);
                }
            }

            Validate(result, err => ValidateIntegrationInfo(externalIntegrationSettings));
            Validate(result, err => ValidateMerchantSettings(externalIntegrationSettings));

            return result;
        }

        private static List<ExternalIntegrationErrorDTO> ValidateMerchantSettings(
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;
            if (externalIntegrationSettings.MerchantSettings == null ||
                externalIntegrationSettings.MerchantSettings.Count == 0)
            {
                AddError(ref errors, $"The '{nameof(externalIntegrationSettings.MerchantSettings)}' settings are missing.");
                return errors;
            }

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
    }
}