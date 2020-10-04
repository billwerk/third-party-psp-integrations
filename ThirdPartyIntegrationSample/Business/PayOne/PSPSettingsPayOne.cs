using System;
using System.Collections.Generic;
using System.Linq;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.IntegrationInfo;
using Business.PayOne.Helpers;
using Business.PayOne.Model.Enums;

namespace Business.PayOne
{
    public class PSPSettingsPayOne : PSPSettings
    {
        public PSPSettingsPayOne(IList<MerchantSettingValue> merchantSettings)
        {
            MerchantId = GetValueByKey(merchantSettings, PayOneConstants.MerchantIdKey);
            AccountId = GetValueByKey(merchantSettings, PayOneConstants.AccountIdKey);
            PortalId = GetValueByKey(merchantSettings, PayOneConstants.PortalIdKey);
            Key = GetValueByKey(merchantSettings, PayOneConstants.KeyKey);
            PortalIdRecurring = GetValueByKey(merchantSettings, PayOneConstants.PortalIdRecurringKey);
            KeyRecurring = GetValueByKey(merchantSettings, PayOneConstants.KeyRecurringKey);
        }
        
        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string PortalId { get; set; }
        public string Key { get; set; }
        public string PortalIdRecurring { get; set; }
        public string KeyRecurring { get; set; }
        public PayOneMode Mode { get; set; }
        
        private static string GetValueByKey(IEnumerable<MerchantSettingValue> merchantSettings, string key)
        {
            return merchantSettings
                .First(s => string.Equals(s.KeyName, key, StringComparison.InvariantCultureIgnoreCase)).KeyValue;
        }
    }
}