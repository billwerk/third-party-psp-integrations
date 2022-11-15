// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.Modules.TransferObjects;

namespace Reepay.SDK.Models.RecurringSession;

/// <summary>
/// https://docs.reepay.com/reference/createrecurringsession
/// </summary>
/// <param name="Id">Session id</param>
/// <param name="Url">Session URL</param>
public record RecurringSessionResponse(
    string Id,
    string Url)
{
    /// <summary>
    /// Builds a PreauthResponseDto
    /// </summary>
    /// <returns></returns>
    public InitialResponse ToPreauthResponseDto() => new(new PreauthResponseDto
        {
            Status = PaymentTransactionStatus.Initiated,
            LastUpdated = DateTime.UtcNow,
            RedirectUrl = Url,
        },
        null);
}
