using System.Collections.Generic;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;

namespace Business
{
    public abstract class PSPSettings
    {
        protected PSPSettings(IList<MerchantSettingValue> merchantSettings)
        {
            
        }
    }
}