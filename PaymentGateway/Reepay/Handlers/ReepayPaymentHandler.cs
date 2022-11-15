// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Shared;
using PaymentGateway.Application;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Modules.Transactions.Agreement;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.SDK.Models.Charges;

namespace Reepay.Handlers;

public class ReepayPaymentHandler : ReepayHandlerBase, IPspPaymentHandler
{
    public ReepayPaymentHandler(
        ISettingsRepository settingsRepository,
        IFlurlClientFactory flurlClientFactory)
        : base(settingsRepository, flurlClientFactory)
    {
    }

    public async Task<Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto>> SendPaymentAsync(PaymentRequestDto paymentRequestDto, Agreement agreement) =>
        (await new CreateChargeRequest(paymentRequestDto, agreement)
            .To(Wrapper.CreateCharge))
        .Bind(charge => charge.ToPaymentResponse(paymentRequestDto.TransactionId, agreement));
}
