// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo;
using PaymentGateway.Application.BillwerkSDK.DTO.IntegrationInfo.Responses;
using PaymentGateway.Domain.Modules;
using PaymentGateway.Domain.Modules.PSP.Enums;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace Reepay;

public class ReepaySettings : PspSettings, ICreditCardPspSettings
{

    public override PaymentProvider PaymentProvider => PaymentProvider.Reepay;

    public NotEmptyString WebhookSecret { get; init; }

    public NotEmptyString PrivateKey { get; set; }

    public CreditCardTypes[] AvailableCreditCardTypes { get; init; }

    private const decimal MinimumDefaultPreauthAmount = Decimal.Zero;
    
    public static IntegrationInfoResponseDto IntegrationInfoResponse => new()
    {
        SupportRefunds = true,
        UsePaymentDataConfirmationFlow = false,
        UsePaymentDataConfirmationFlowForPreauth = false,
        UsesScheduledPayment = false,
        SupportMultipleTransactionOverpayments = false,
        RequiresReturnUrl = true,
        CreditCardMethodInfo = new PaymentMethodInfoDto
        {
            UseCapturePreauth = true,
            UseCancelPreauth = false,
            DefaultPreauthAmount = MinimumDefaultPreauthAmount,
        },
        MerchantSettings = new List<MerchantSettingDescription>
        {
            new()
            {
                DisplayName = "Private key",
                KeyName = "PrivateKey",
                PlaceHolder = "Private key",
                Required = true,
            },
        },
        HasSupportInitialBearer = false,
        HasSupportOrderDataInPreauth = true,
        HasSupportInvoiceData = false,
    };
}
