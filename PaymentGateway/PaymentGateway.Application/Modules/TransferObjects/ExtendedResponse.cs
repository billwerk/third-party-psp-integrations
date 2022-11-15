// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Interfaces;

namespace PaymentGateway.Application.Modules.TransferObjects;

public record ExtendedResponse<T>(
    T Response,
    IDictionary<string, string>? PspTransactionData = null)
    where T : ITransactionResponseDto
{
    public static implicit operator ExtendedResponse<T>(T value) => new (value);
}
