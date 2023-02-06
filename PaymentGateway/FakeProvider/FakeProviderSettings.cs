// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Settings;

namespace FakeProvider;

public class FakeProviderSettings : PspSettings
{
    //Here can be placed specific Payment Provider data related to communication with them
    //Like API tokens, bearers, urls and so on.

    public string ApiToken { get; set; }

    public string MerchantId { get; set; }

    public static IntegrationInfoResponseDto IntegrationInfoResponse => new()
    {
        SupportRefunds = true,
        DebitMethodInfo = new PaymentDirectDebitMethodInfoDto
        {
            UnconfirmedLedger = true,
            SupportExternalMandateReference = true,
            SupportLowerCaseMandateReference = true,
            SupportBackendPayments = true,
            HasSupportInitialBearer = true,
        },
        MerchantSettings = new List<MerchantSettingDescription>
        {
            new()
            {
                DisplayName = "API token",
                KeyName = nameof(ApiToken),
                Required = true,
                PlaceHolder = "Value to pass validation: '12345'",
            },
            new()
            {
                DisplayName = "Merchant Id",
                KeyName = nameof(MerchantId),
                Required = true,
                PlaceHolder = "Pass any string",
            },
        },
        HasSupportInitialBearer = true,
    };
    
    public override PaymentProvider PaymentProvider => PaymentProvider.FakeProvider;
}
