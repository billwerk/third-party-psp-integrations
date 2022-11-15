// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Modules.PSP.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;
using Reepay.Helpers;
using Reepay.SDK.Models.Refund;

namespace Reepay.Handlers;

public class ReepayRefundHandler : ReepayHandlerBase, IPspRefundHandler
{
    public ReepayRefundHandler(
        ISettingsRepository settingsRepository,
        IFlurlClientFactory flurlClientFactory)
        : base (settingsRepository, flurlClientFactory)
    {
    }

    public async Task<Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto>> SendRefundAsync(NotEmptyString pspPaymentId, RefundRequestDto refundRequestDto, IDictionary<string, string> pspTransactionData)
    {
        var invoiceHandle = pspTransactionData[ReepayConstants.PspTransactionDataInvoiceHandle];

        return (await new CreateRefundRequest(invoiceHandle, refundRequestDto.RequestedAmount.ToReepayAmount(refundRequestDto.Currency))
                .To(Wrapper.CreateRefund))
            .Bind(response => response.ToRefundResponse(refundRequestDto));
    }
}
