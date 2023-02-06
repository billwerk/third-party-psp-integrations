// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Application.Modules.Settings;
using PaymentGateway.Domain.BillwerkSDK.Settings;

namespace PaymentGateway.Settings;

[Route("/api")]
public class PspSettingsController : ControllerBase
{
    private readonly ISettingsHandler _settingsHandler;

    public PspSettingsController(ISettingsHandler settingsHandler)
    {
        _settingsHandler = settingsHandler;
    }

    [HttpGet("IntegrationInfo")]
    public ActionResult<IntegrationInfoResponseDto> FetchIntegrationInfo()
        => _settingsHandler.FetchIntegrationInfo();
    
    [HttpGet("Settings")]
    public ActionResult<ThirdPartyMerchantPspSettings> FetchSettings()
        => new(_settingsHandler.FetchSettings());

    [HttpPut("Settings")]
    public async Task<ActionResult<IntegrationSettingsResponseDto>> UpdateSettings([FromBody]ThirdPartyMerchantPspSettings thirdPartyMerchantPspSettings)
    {
        var pspSettingsVerificationResult = await _settingsHandler.VerifyPspSettingsAsync(thirdPartyMerchantPspSettings);

        if (pspSettingsVerificationResult.IsFailure)
            return UnprocessableEntity(pspSettingsVerificationResult.Error);

        _settingsHandler.SaveSettings(thirdPartyMerchantPspSettings);

        return new ActionResult<IntegrationSettingsResponseDto>(pspSettingsVerificationResult.Data);
    }
    
}
