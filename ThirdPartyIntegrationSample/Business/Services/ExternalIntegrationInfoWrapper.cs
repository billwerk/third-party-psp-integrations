using System;
using Billwerk.Payment.PayOne.Services;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.Enums;
using Business.Interfaces;

namespace Business.Factory
{
    
    public class ExternalIntegrationInfoWrapper: IExternalIntegrationInfoWrapper
    {
        public ExternalIntegrationInfoDTO Create(PaymentServiceProvider provider)
        {
            switch (provider)
            {
                case PaymentServiceProvider.PayOne:
                    return new PayOneExternalIntegrationInfoService().Create();
                default:
                    throw new NotSupportedException($"Provide={provider} is not supported!");
            }
        }
    }
}