// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Requests;

namespace PaymentGateway.Domain.BillwerkSDK.Settings
{
    /// <summary>
    /// Implementation of <see cref="IMerchantPspSettings"/> which used in
    /// <see cref="PaymentSettingsRequestDto"/> request model.
    /// </summary>
    public class ThirdPartyMerchantPspSettings
    {
        /// <summary>
        /// List of key/value merchant settings elements.
        /// Mandatory.
        /// </summary>
        public List<MerchantSettingValue> MerchantSettings { get; set; }
    }

}
