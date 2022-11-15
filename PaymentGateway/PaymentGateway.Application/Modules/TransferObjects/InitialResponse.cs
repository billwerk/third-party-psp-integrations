// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;

namespace PaymentGateway.Application.Modules.TransferObjects;

/// <summary>
/// Probably instead of this field we need to add AgreementResponseDto to PreauthResponseDto and remove duplicate fields.
/// Can be applied with some other PG issue, but it will require some changes for preauth handling for external providers in billwerk.
/// </summary>
public record InitialResponse(
        PreauthResponseDto PreauthResponseDto,
        string? PspAgreementId,
        DateTime? BookingDate = null,
        IDictionary<string, string>? PspTransactionData = null)
    : ExtendedResponse<PreauthResponseDto>(PreauthResponseDto, PspTransactionData);
