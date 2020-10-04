using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;

namespace Business.Interfaces
{
    public interface IExternalSettingsValidator
    {
        ExternalIntegrationValidateSettingsDTO Validate(ExternalIntegrationValidateSettingsRequestDTO externalIntegrationSettings);
    }
}