// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;

public interface ISupportCapturePreauth : IPspHandler
{
    Task<Result<ExtendedResponse<PaymentResponseDto>, PaymentErrorDto>> CapturePreauthAsync(PaymentRequestDto paymentRequestDto, Domain.Modules.Transactions.Agreement.Agreement agreement, NotEmptyString preauthPspTransactionId);
}
