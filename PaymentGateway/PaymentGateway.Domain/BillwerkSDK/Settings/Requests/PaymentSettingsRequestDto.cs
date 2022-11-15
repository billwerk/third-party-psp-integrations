using Billwerk.Payment.SDK.DTO.IntegrationInfo;
using PaymentGateway.Domain.BillwerkSDK.Settings;

namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Requests
{
    /// <summary>
    /// Model for creating/updating merchant psp settings.
    /// </summary>
    public class PaymentSettingsRequestDto
    {
        /// <summary>
        /// Here used interface for internal billwerk usage.
        /// Integrator must expect <see cref="ThirdPartyMerchantPspSettings"/> implementation here.
        /// Mandatory.
        /// </summary>
        public IMerchantPspSettings MerchantPspSettings { get; set; }
    }
}
