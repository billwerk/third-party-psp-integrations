using System;
using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Interfaces;

namespace Business.PayOne.Services
{
    public class PayOneExternalSettingsValidator : ExternalSettingsValidatorBase, IExternalSettingsValidator
    {
        public PayOneExternalSettingsValidator(IExternalIntegrationInfoFactory externalIntegrationInfoFactory) 
            : base(externalIntegrationInfoFactory)
        {
        }

        public ExternalIntegrationValidateSettingsDTO Validate(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var errors = ValidateDto(externalIntegrationSettings);

            return new ExternalIntegrationValidateSettingsDTO
            {
                Errors = errors
            };
        }
        
        private List<ExternalIntegrationErrorDTO> ValidateDto(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            var result = new List<ExternalIntegrationErrorDTO>();
           
            void Validate(List<ExternalIntegrationErrorDTO> errors, Func<ExternalIntegrationValidateSettingsRequestDTO, List<ExternalIntegrationErrorDTO>> validateFunc)
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

        private List<ExternalIntegrationErrorDTO> ValidateMerchantSettings(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
           return null;
        }
    }
}