// Copyright (c) billwerk GmbH. All rights reserved

using System.Net;
using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.Enums;
using Reepay.SDK.Enums;
using static Reepay.Helpers.ReepayMappingHelper;

namespace Reepay.SDK.Models.Errors;

public record ErrorResponse(
    ReepayErrorCode Code,
    string Error,
    string? Message,
    HttpStatusCode HttpStatus,
    string HttpReason,
    string Path,
    DateTime Timestamp,
    string RequestId,
    TransactionError? TransactionError)
{
    public PaymentErrorDto ToPaymentErrorDto() => new()
    {
        ErrorCode = Code.ToPaymentErrorCode(),
        ErrorMessage = string.IsNullOrEmpty(Message) ? Error : $"{Error}: {Message}",
        PspErrorCode = ((int)Code).ToString(),
        ReceivedAt = DateTime.UtcNow,
        Status = PaymentTransactionStatus.Failed,
    };
}
