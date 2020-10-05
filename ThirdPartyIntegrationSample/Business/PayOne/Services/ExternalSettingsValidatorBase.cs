using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Interfaces;
using Business.PayOne.Helpers;
using Core.Helpers;

namespace Business.PayOne.Services
{
    public abstract class ExternalSettingsValidatorBase
    {
        private readonly IExternalIntegrationInfoFactory _externalIntegrationInfoFactory;

        protected ExternalSettingsValidatorBase(IExternalIntegrationInfoFactory externalIntegrationInfoFactory)
        {
            _externalIntegrationInfoFactory = externalIntegrationInfoFactory;
        }

        protected List<ExternalIntegrationErrorDTO> ValidateIntegrationInfo(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;
            var originalExternalIntegrationInfoSettings = _externalIntegrationInfoFactory.Create();
            if (ObjectsComparator.AreObjectsEqual(originalExternalIntegrationInfoSettings, externalIntegrationSettings.IntegrationInfo) == false)
            {
                AddError(ref errors, $"The '{nameof(externalIntegrationSettings.IntegrationInfo)}' settings are different from original.");
            }
            
            return errors;
        }
        
        protected static List<ExternalIntegrationErrorDTO> ValidatePrenotificationDays(
            ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings)
        {
            List<ExternalIntegrationErrorDTO> errors = null;
            if (externalIntegrationSettings.InitialDDPrenotificationDays <
                PayOneConstants.DirectDebitInitialPaymentProcessingDays)
            {
                AddError(ref errors,
                    $"Value for InitialDDPrenotificationDays={externalIntegrationSettings.InitialDDPrenotificationDays} is too small, minimal value is = {PayOneConstants.DirectDebitInitialPaymentProcessingDays}");
            }

            if (externalIntegrationSettings.RecurringDDPrenotificationDays <
                PayOneConstants.DirectDebitRecurringPaymentProcessingDays)
            {
                AddError(ref errors,
                    $"Value for RecurringDDPrenotificationDays={externalIntegrationSettings.RecurringDDPrenotificationDays} is too small, minimal value is = {PayOneConstants.DirectDebitRecurringPaymentProcessingDays}");
            }

            return errors;
        }
        
        protected static void AddError(ref List<ExternalIntegrationErrorDTO> errors, string errorMessage)
        {
            errors ??= new List<ExternalIntegrationErrorDTO>();

            errors.Add(new ExternalIntegrationErrorDTO
            {
                ErrorMessage = errorMessage
            });
        }
    }
}