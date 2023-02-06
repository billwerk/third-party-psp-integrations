// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Settings;

public interface ISettingsHandler
{
    /// <summary>
    /// Verify provided specific PSP settings, like API tokens, creds and etc.
    /// </summary>
    /// <param name="thirdPartyMerchantPspSettings">Merchant settings in dynamic form</param>
    /// <returns>Result of verification. If is not success, error dto.</returns>
    Task<Result<IntegrationSettingsResponseDto, PaymentErrorDto>> VerifyPspSettingsAsync(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings);

    /// <summary> 
    /// Save PSP settings, based on changes provided by merchant.
    /// </summary>
    /// <param name="thirdPartyMerchantPspSettings"></param>
    void SaveSettings(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings);

    /// <summary>
    /// Fetch editable by merchant PSP settings.
    /// </summary>
    /// <returns></returns>
    ThirdPartyMerchantPspSettings FetchSettings();
    
    /// <summary>
    /// Fetch integration info - data needed for billwerk, according to PSP-specific behaviour.
    /// </summary>
    /// <returns></returns>
    IntegrationInfoResponseDto FetchIntegrationInfo();
}
