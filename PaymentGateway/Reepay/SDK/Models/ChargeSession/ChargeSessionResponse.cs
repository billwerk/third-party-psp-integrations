// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.Modules.TransferObjects;

namespace Reepay.SDK.Models.ChargeSession;

/// <summary>
/// https://docs.reepay.com/reference/createchargesession
/// </summary>
public record ChargeSessionResponse(
    string Id,
    string Url)
{
    /// <summary>
    /// Builds a PreauthResponseDto
    /// </summary>
    /// <returns></returns>
    public InitialResponse ToPreauthResponse() => new(new PreauthResponseDto
        {
            Status = PaymentTransactionStatus.Initiated,
            LastUpdated = DateTime.UtcNow,
            RedirectUrl = Url,
        },
        null);
};
