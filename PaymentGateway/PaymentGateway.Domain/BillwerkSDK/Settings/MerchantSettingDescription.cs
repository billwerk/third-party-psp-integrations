// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo
{
    /// <summary>
    /// Metadata of merchant settings element.
    /// Use for first setup of settings.
    /// </summary>
    public class MerchantSettingDescription
    {
        /// <summary>
        /// UI name of element.
        /// Mandatory.
        /// </summary>
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Name of element key.
        /// Mandatory.
        /// </summary>
        public string KeyName { get; set; }
       
        /// <summary>
        /// Indicate, if this merchant settings element must be filled.
        /// Mandatory.
        /// </summary>
        public bool Required { get; set; }
        
        /// <summary>
        /// Set of possible values for merchant settings element, if this is can be only one value from
        /// predefined set of values (will be display as select input with options).
        /// Optional.
        /// </summary>
        public SortedDictionary<string, string> PredefinedValues { get; set; }
        
        /// <summary>
        /// Placeholder for UI input of merchant settings element.
        /// Optional.
        /// </summary>
        public string PlaceHolder { get; set; }
    }
}
