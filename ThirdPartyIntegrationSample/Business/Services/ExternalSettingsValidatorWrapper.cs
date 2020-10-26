using System;
using System.Collections.Generic;
using System.Linq;
using Billwerk.Payment.PayOne.Services;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Billwerk.Payment.SDK.Interfaces;
using Business.Enums;
using Business.Interfaces;
using Core.Helpers;

namespace Business.PayOne.Services
{
    public class ExternalSettingsValidatorWrapper: IExternalSettingsValidatorWrapper
    {
        private readonly IExternalIntegrationInfoWrapper _externalIntegrationInfo;

        private readonly PayOneExternalSettingsValidator _payOneExternalSettingsValidator;
        

        protected ExternalSettingsValidatorWrapper(IExternalIntegrationInfoWrapper externalIntegrationInfo, PayOneExternalSettingsValidator payOneExternalSettingsValidator)
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
            var service=GetValidatorService(provider);
            return service.ValidateIntegrationInfo(externalIntegrationSettings);
        }

        private List<ExternalIntegrationErrorDTO> ValidateMerchantSettings(PaymentServiceProvider provider, ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;
            if (externalIntegrationSettings.MerchantSettings == null ||
                externalIntegrationSettings.MerchantSettings.Count == 0)
            {
                AddError(ref errors, $"The '{nameof(externalIntegrationSettings.MerchantSettings)}' settings are missing.");
                return errors;
            }
            var service=GetValidatorService(provider);
            return service.ValidateMerchantSettings(externalIntegrationSettings);
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
            var errors = ValidateDto(provider, externalIntegrationSettings);
            var hasErrors = errors != null && errors.Count > 0;
            
            return new ExternalIntegrationValidateSettingsDTO
            {
                IsCreditCardValid = !hasErrors,
                IsDebitValid = !hasErrors,
                IsOnAccountValid = !hasErrors,
                Errors = errors
            };
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

        private List<ExternalIntegrationErrorDTO> ValidateDto(PaymentServiceProvider provider,
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var result = new List<ExternalIntegrationErrorDTO>();

            void Validate(List<ExternalIntegrationErrorDTO> errors,
                Func<PaymentServiceProvider, ExternalIntegrationValidateSettingsRequestDTO, List<ExternalIntegrationErrorDTO>> validateFunc)
            {
                var validationErrors = validateFunc(provider, externalIntegrationSettings);
                if (validationErrors != null)
                {
                    errors.AddRange(validationErrors);
                }
            }

            Validate(result, ValidateIntegrationInfo);
            Validate(result, ValidateMerchantSettings);

            return result;
        }        
    }
}