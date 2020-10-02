using Business.PayOne.Model;
using Business.PayOne.Model.Enums;

namespace Business.PayOne
{
    public class PSPSettingsPayOne : PSPSettings
    {
        public PSPSettingsPayOne( )
        {
            
        }

        public string MerchantId { get; set; }
        public string AccountId { get; set; }
        public string PortalId { get; set; }
        public string Key { get; set; }
        public string PortalIdRecurring { get; set; }
        public string KeyRecurring { get; set; }
        public PayOneMode Mode { get; set; }
    }
}