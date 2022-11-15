// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using MediatR;
using PaymentGateway.Application.BillwerkSDK.DTO.Requests;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.Transactions.Preauth.FinalizePreauth;

public class FinalizePreauthCommand : IRequest<Result<PreauthResponseDto, PaymentErrorDto>>
{
    public BillwerkTransactionId PreauthTransactionId { get; }

    public IDictionary<string, string> FinalizationData { get; }

    public FinalizePreauthCommand(FinalizePreauthRequestDto finalizePreauthRequestDto)
    {
        PreauthTransactionId = new BillwerkTransactionId(finalizePreauthRequestDto.TransactionId);
        FinalizationData = finalizePreauthRequestDto.FinalizationData;
    }
}
