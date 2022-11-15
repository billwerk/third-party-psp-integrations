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
    
    public static IntegrationInfoResponseDto IntegrationInfoResponse => new()
    {
        SupportRefunds = true,
        DebitMethodInfo = new PaymentDirectDebitMethodInfoDto
        {
            UnconfirmedLedger = true,
            SupportExternalMandateReference = true,
            SupportLowerCaseMandateReference = true,
            SupportBackendPayments = true,
        },
        MerchantSettings = new List<MerchantSettingDescription>(),
        HasSupportInitialBearer = true,
    };
    
    public override PaymentProvider PaymentProvider => PaymentProvider.FakeProvider;
}
