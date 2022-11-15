// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Shared;
using PaymentGateway.Application;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Helpers;
using Reepay.SDK.Models.Charges;
using Reepay.SDK.Models.ChargeSession;
using Reepay.SDK.Models.RecurringSession;

namespace Reepay.Handlers;

public class ReepayPreauthHandler : ReepayHandlerBase, IPspPreauthHandler, ISupportFinalizePreauth, ISupportCapturePreauth
{
    public ReepayPreauthHandler(
        ISettingsRepository settingsRepository,
        IFlurlClientFactory flurlClientFactory)
        : base(settingsRepository, flurlClientFactory)
    {
    }

    public Task<Result<InitialResponse, PaymentErrorDto>> SendInitialPreauthAsync(PreauthRequestDto preauthRequestDto) =>
        preauthRequestDto.RequestedAmount > 0
            ? CreateChargeSessionAsync(preauthRequestDto)
            : CreateRecurringSessionAsync(preauthRequestDto);

    public async Task<Result<InitialResponse, PaymentErrorDto>> SendUpgradePreauthAsync(PreauthRequestDto preauthRequestDto, Agreement agreement) =>
        (await preauthRequestDto
            .Do(ValidatePreauthRequest)
            .To(dto => new CreateChargeRequest(dto, agreement))
            .To(Wrapper.CreateCharge))
        .Bind(response => response.ToPreauthResponse(preauthRequestDto.TransactionId));
    

    public Task<Result<PaymentCancellationResponseDto, PaymentErrorDto>> CancelPreauthAsync(NotEmptyString? pspTransactionId) =>
        throw new NotImplementedException("Reepay PSP doesn't support preauth cancellation mechanism - their preauth " +
                                          "transactions cancelled automatically.");

    public async Task<Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto>> CapturePreauthAsync(PaymentRequestDto paymentRequestDto, Agreement agreement, NotEmptyString preauthPspTransactionId) =>
        (await Wrapper.SettleCharge(paymentRequestDto.PreauthTransactionId))
        .Bind(charge => charge.ToPaymentResponse(paymentRequestDto.TransactionId, agreement));

    public Task<Result<InitialResponse, PaymentErrorDto>> FinalizePreauthAsync(
        NotEmptyString? pspTransactionId,
        IDictionary<string, object> finalizationData) =>
        IsPreauthAmountEqualToZero(finalizationData)
            ? FinalizeZeroPreauthAsync(finalizationData)
            : FinalizeNonZeroPreauthAsync(finalizationData);

    private Task<Result<InitialResponse, PaymentErrorDto>> CreateChargeSessionAsync(PreauthRequestDto preauthRequestDto) =>
        preauthRequestDto
            .Do(ValidatePreauthRequest)
            .To(dto => new CreateChargeSessionRequest(dto, Settings.AvailableCreditCardTypes.ToPaymentMethods()))
            .To(Wrapper.CreateChargeSession)
            .MapResultAsync(response => response.ToPreauthResponse());

    private Task<Result<InitialResponse, PaymentErrorDto>> CreateRecurringSessionAsync(PreauthRequestDto preauthRequestDto) =>
        preauthRequestDto
            .Do(ValidatePreauthRequest)
            .To(dto => new CreateRecurringSessionRequest(dto, Settings.AvailableCreditCardTypes.ToPaymentMethods()))
            .To(Wrapper.CreateRecurringSession)
            .MapResultAsync(response => response.ToPreauthResponseDto());

    private static bool IsPreauthAmountEqualToZero(IDictionary<string, object> finalizationData) =>
        finalizationData.TryGetValue(nameof(PreauthRequestDto.RequestedAmount), out var value)
        && value is NonNegativeAmount
        {
            Value: 0,
        };

    private async Task<Result<InitialResponse, PaymentErrorDto>> FinalizeNonZeroPreauthAsync(IDictionary<string, object> finalizationData) =>
        (await Wrapper.GetCharge((string)finalizationData["invoice"]))
        .Bind(response => response.ToPreauthResponse((string)finalizationData["pactasTransactionId"]));

    private async Task<Result<InitialResponse, PaymentErrorDto>> FinalizeZeroPreauthAsync(IDictionary<string, object> finalizationData) =>
        (await Wrapper.GetPaymentMethod((string)finalizationData["payment_method"]))
        .Bind(paymentMethod =>
            paymentMethod.ToPreauthResponse((string)finalizationData["pactasTransactionId"],
                (Currency)finalizationData["Currency"]));

    private static void ValidatePreauthRequest(PreauthRequestDto preauthRequestDto)
    {
        if (preauthRequestDto.PaymentMeansReference.Role != PublicPaymentProviderRole.CreditCard)
            throw new NotSupportedException($"{preauthRequestDto.PaymentMeansReference.Role} is not supported by Reepay");
    }
}
