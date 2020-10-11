using Core.Helpers;

namespace Business.PayOne.Model.Requests
{
    public abstract class RequestBase : ModelBase
    {
        protected RequestBase(bool initialPayment, PayOnePSPSettings settings)
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
        public string Integrator_Name => "billwerk.thirdparty";
        public string Integrator_Version => "v.1.0";
        public string Solution_Name => "billwerk.thirdparty";
        public string Solution_Version => "v.1.0";
    }
}