using Core.Helpers;

namespace Business.PayOne.Model.Requests
{
    public abstract class RequestBase
    {
        protected RequestBase(bool initialPayment, PSPSettingsPayOne settings)
        {
            MId = settings.MerchantId;
            PortalId = initialPayment ? settings.PortalId : settings.PortalIdRecurring;
            Key = HashCalculator.GetMd5(initialPayment ? settings.Key : settings.KeyRecurring);
            Mode = settings.Mode.ToString("G").ToLowerInvariant();
        }

        public string Request => GetType().Name.ToLowerInvariant();
        public string MId { get; }
        public string PortalId { get; set; }
        public string Key { get; set; }
        public string Mode { get; set; }
        public string Encoding => "UTF-8";
        public string IntegratorName => "billwerk.thirdparty";
        public string IntegratorVersion => "v.1.0";
        public string SolutionName => "billwerk.thirdparty";
        public string SolutionVersion => "v.1.0";
    }
}