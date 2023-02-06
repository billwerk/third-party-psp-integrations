// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.PSP;

public interface IPspSettingsHandler : IPspHandler
{
    Task<Result<IntegrationSettingsResponseDto, PaymentErrorDto>> VerifyPspSettingsAsync(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings);
    
    void SaveSettings(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings);

    ThirdPartyMerchantPspSettings FetchSettings();

    IntegrationInfoResponseDto FetchIntegrationInfo();
}
