// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using FakeProvider.Handlers;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Shared;

namespace FakeProvider;

public class FakeProviderSettingsHandler : FakeProviderHandlerBase, IPspSettingsHandler
{
    private readonly ISettingsRepository _settingsRepository;
    
    public FakeProviderSettingsHandler(
        ISettingsRepository settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }
    
    public Task<Result<IntegrationSettingsResponseDto, PaymentErrorDto>> VerifyPspSettingsAsync(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
    {
        var apiToken = thirdPartyMerchantPspSettings.MerchantSettings
            .FirstOrDefault(x => x.KeyName == nameof(FakeProviderSettings.ApiToken));
        
        var merchantId = thirdPartyMerchantPspSettings.MerchantSettings
            .FirstOrDefault(x => x.KeyName == nameof(FakeProviderSettings.MerchantId));

        if (apiToken is null || apiToken.KeyValue != "12345")
        {
           return Result<IntegrationSettingsResponseDto, PaymentErrorDto>
                .Failure(new PaymentErrorDto() { ErrorMessage = "Invalid Api Token." +
                                                                "For Fake Provider valid token is '12345'" })
                .To(Task.FromResult);
        }

        if (merchantId is null || string.IsNullOrEmpty(merchantId.KeyValue))
        {
          return Result<IntegrationSettingsResponseDto, PaymentErrorDto>
                .Failure(new PaymentErrorDto
                { ErrorMessage = "Merchant Id is required. For Fake Provider you can put any not empty value."})
                .To(Task.FromResult);
        }

        var integrationSettingsResponseDto = new IntegrationSettingsResponseDto
        {
            IsDebitValid = true,
        };
        
       return Result<IntegrationSettingsResponseDto, PaymentErrorDto>.Ok(integrationSettingsResponseDto).To(Task.FromResult);
    }
    
    public void SaveSettings(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
    {
        var apiToken = thirdPartyMerchantPspSettings.MerchantSettings
            .FirstOrDefault(x => x.KeyName == nameof(FakeProviderSettings.ApiToken));
        
        var merchantId = thirdPartyMerchantPspSettings.MerchantSettings
            .FirstOrDefault(x => x.KeyName == nameof(FakeProviderSettings.MerchantId));

        if (!string.IsNullOrEmpty(apiToken?.KeyValue) && !string.IsNullOrEmpty(merchantId?.KeyValue))
        {
            var settings = _settingsRepository.GetDefault(PaymentProvider.FakeProvider) as FakeProviderSettings;
            settings.ApiToken = apiToken.KeyValue;
            settings.MerchantId = merchantId.KeyValue;
            _settingsRepository.SaveSettings(settings);
        }
    }

    public ThirdPartyMerchantPspSettings FetchSettings()
    {
        var settings = _settingsRepository.GetDefault(PaymentProvider.FakeProvider) as FakeProviderSettings;
        
        var merchantSettings = new List<MerchantSettingValue>
        {
            new() {  KeyName = nameof(FakeProviderSettings.ApiToken), KeyValue = settings.ApiToken},
            new() {  KeyName = nameof(FakeProviderSettings.MerchantId), KeyValue = settings.MerchantId},
        };

        return new ThirdPartyMerchantPspSettings { MerchantSettings = merchantSettings };
    }
    
    public IntegrationInfoResponseDto FetchIntegrationInfo() => FakeProviderSettings.IntegrationInfoResponse;
}
