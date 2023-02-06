// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Handlers;

namespace Reepay;

public class ReepaySettingsHandler : ReepayHandlerBase, IPspSettingsHandler
{
    private readonly ISettingsRepository _settingsRepository;

    public ReepaySettingsHandler(
        ISettingsRepository settingsRepository,
        IFlurlClientFactory flurlClientFactory)
        : base(settingsRepository, flurlClientFactory)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<Result<IntegrationSettingsResponseDto, PaymentErrorDto>> VerifyPspSettingsAsync(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
    {
        var privateKey = thirdPartyMerchantPspSettings.MerchantSettings.FirstOrDefault(x => x.KeyName == nameof(ReepaySettings.PrivateKey));
        var webhookSecret = thirdPartyMerchantPspSettings.MerchantSettings.FirstOrDefault(x => x.KeyName == nameof(ReepaySettings.WebhookSecret));
        Settings.PrivateKey = new NotEmptyString(privateKey.KeyValue);
        var webhookSettingAsResult = await Wrapper.GetWebhookSettingsAsync();

        if (webhookSettingAsResult.IsFailure)
            return new IntegrationSettingsResponseDto { IsCreditCardValid = false };

        var isMerchantWebhookSecretEqualToFetched = webhookSettingAsResult.Data.Secret == webhookSecret?.KeyValue;

        return isMerchantWebhookSecretEqualToFetched
            ? new IntegrationSettingsResponseDto { IsCreditCardValid = true } 
            : new IntegrationSettingsResponseDto { IsCreditCardValid = false };
    }
    
    public void SaveSettings(ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
    {
        var privateKey = thirdPartyMerchantPspSettings.MerchantSettings.FirstOrDefault(x => x.KeyName ==  nameof(ReepaySettings.PrivateKey));
        var webhookSecret = thirdPartyMerchantPspSettings.MerchantSettings
            .FirstOrDefault(x => x.KeyName == nameof(ReepaySettings.WebhookSecret));
        
        if (privateKey is not null && !string.IsNullOrEmpty(privateKey.KeyValue))
        {
            Settings.PrivateKey = new NotEmptyString(privateKey.KeyValue);
            Settings.WebhookSecret = new NotEmptyString(webhookSecret.KeyValue);
            _settingsRepository.SaveSettings(Settings);
        }
    }

    public ThirdPartyMerchantPspSettings FetchSettings()
    {
        var settings = _settingsRepository.GetDefault() as ReepaySettings;
        
        var merchantSettings = new List<MerchantSettingValue>
        {
            new() {  KeyName = nameof(ReepaySettings.PrivateKey), KeyValue = settings.PrivateKey},
            new() {  KeyName = nameof(ReepaySettings.WebhookSecret), KeyValue = settings.WebhookSecret},
        };

        return new ThirdPartyMerchantPspSettings { MerchantSettings = merchantSettings };
    }

    public IntegrationInfoResponseDto FetchIntegrationInfo() => ReepaySettings.IntegrationInfoResponse;
}

