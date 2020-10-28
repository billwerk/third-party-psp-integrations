using System;
using System.Collections.Generic;
using Billwerk.Payment.PayOne.Services;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Billwerk.Payment.SDK.Interfaces;
using Business.Enums;
using Business.Interfaces;
using Core.Helpers;

namespace Business.Services
{
    public class ExternalSettingsValidatorWrapper: IExternalSettingsValidatorWrapper
    {
        private readonly IExternalIntegrationInfoWrapper _externalIntegrationInfo;

        private readonly PayOnePspSettingsValidator _payOneExternalSettingsValidator;

        protected ExternalSettingsValidatorWrapper(IExternalIntegrationInfoWrapper externalIntegrationInfo, PayOnePspSettingsValidator payOneExternalSettingsValidator)
        {
            _externalIntegrationInfo = externalIntegrationInfo;
            _payOneExternalSettingsValidator = payOneExternalSettingsValidator;
        }

        private List<ExternalIntegrationErrorDTO> ValidateIntegrationInfo(PaymentServiceProvider provider, ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;
            var originalExternalIntegrationInfoSettings = _externalIntegrationInfo.Create(provider);
            if (ObjectsComparator.AreObjectsEqual(originalExternalIntegrationInfoSettings, externalIntegrationSettings.IntegrationInfo) == false)
            {
                AddError(ref errors, $"The '{nameof(externalIntegrationSettings.IntegrationInfo)}' settings are different from original.");
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

        public ExternalIntegrationValidateSettingsDTO Validate(PaymentServiceProvider provider,
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var validationErrors = ValidateIntegrationInfo(provider, externalIntegrationSettings);
            if (validationErrors != null && validationErrors.Count>0)
            {
                return new ExternalIntegrationValidateSettingsDTO
                {
                    Errors = validationErrors
                };
            }
            var service=GetValidatorService(provider);
            return service.Validate(externalIntegrationSettings);
        }

        private IExternalSettingsValidator GetValidatorService(PaymentServiceProvider provider)
        {
            switch (provider)
            {
                case PaymentServiceProvider.PayOne:
                    return _payOneExternalSettingsValidator;
                default:
                    throw new NotSupportedException($"Provide={provider} is not supported!");
            }            
        }

    }
}