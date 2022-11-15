// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Requests;
using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.Modules.PSP;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace FakeProvider.Handlers;

public class FakeProviderRefundHandler : FakeProviderHandlerBase, IPspRefundHandler
{
    public Task<Result<ExtendedResponse<RefundResponseDto>, PaymentErrorDto>> SendRefundAsync(NotEmptyString pspPaymentId, RefundRequestDto refundRequestDto, IDictionary<string, string> pspTransactionData)
        => FakeProviderWrapper.SendRefund(refundRequestDto)
            .BiMap(CreateExtendedResponse, error => error)
            .To(Task.FromResult);
}
