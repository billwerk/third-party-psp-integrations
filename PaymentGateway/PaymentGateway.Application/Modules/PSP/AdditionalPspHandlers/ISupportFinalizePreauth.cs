// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using PaymentGateway.Application.Modules.TransferObjects;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Modules.PSP.AdditionalPspHandlers;

public interface ISupportFinalizePreauth : IPspHandler
{
    Task<Result<InitialResponse, PaymentErrorDto>> FinalizePreauthAsync(NotEmptyString? pspTransactionId, IDictionary<string, object> finalizationData);
}
