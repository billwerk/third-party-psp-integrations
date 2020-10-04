using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;

namespace Business.Interfaces
{
    public interface IExternalIntegrationInfoFactory
    {
        ExternalIntegrationInfoDTO Create();
    }
}