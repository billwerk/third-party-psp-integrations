using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Enums;

namespace Business.Interfaces
{
    public interface IExternalSettingsValidatorWrapper
    {
        ExternalIntegrationValidateSettingsDTO Validate(PaymentServiceProvider provider,ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings);
    }
}