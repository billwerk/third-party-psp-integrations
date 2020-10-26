using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Enums;

namespace Business.Interfaces
{
    
    public interface IExternalIntegrationInfoWrapper
    {
        ExternalIntegrationInfoDTO Create(PaymentServiceProvider provider);
    }
}