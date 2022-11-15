namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo
{
    /// <summary>
    /// Represent single element of merchant settings in key/value manner.
    /// </summary>
    public class MerchantSettingValue
    {
        /// <summary>
        /// Name of merchant settings element key.
        /// Mandatory.
        /// </summary>
        public string KeyName { get; set; }
        
        /// <summary>
        /// Value of merchant settings element.
        /// Mandatory.
        /// </summary>
        public string KeyValue { get; set; }
    }
}
