// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using DryIoc;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Settings;

public class SettingsHandler : ISettingsHandler
{
    private readonly IPspSettingsHandler _pspSettingsHandler;

    public SettingsHandler(IEnumerable<Meta<Lazy<IPspSettingsHandler>, PaymentProvider>> pspPaymentHandlers, PspExecutionContext pspExecutionContext)
        => _pspSettingsHandler = pspPaymentHandlers.Single(meta => meta.Metadata == pspExecutionContext.CurrentPaymentProvider).Value.Value;
    
    public async Task<Result<IntegrationSettingsResponseDto, PaymentErrorDto>> VerifyPspSettingsAsync(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
        => (await _pspSettingsHandler.VerifyPspSettingsAsync(thirdPartyMerchantPspSettings))
            .BiMap(settingsResponse => settingsResponse, e => e);

    public void SaveSettings(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
        => _pspSettingsHandler.SaveSettings(thirdPartyMerchantPspSettings);
    
    public ThirdPartyMerchantPspSettings FetchSettings() => _pspSettingsHandler.FetchSettings();

    public IntegrationInfoResponseDto FetchIntegrationInfo() => _pspSettingsHandler.FetchIntegrationInfo();
}
