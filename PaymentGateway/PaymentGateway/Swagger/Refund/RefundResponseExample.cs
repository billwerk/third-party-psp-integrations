// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Refund;

public class RefundResponseExample : IExamplesProvider<RefundResponseDto>
{
    public RefundResponseDto GetExamples() => new()
    {
        RequestedAmount = 15m,
        RefundedAmount = 15m,
        Currency = "EUR",
        BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
        PspTransactionId = "cf4485e9-ead4-462a-899e-2dfd991e258b",
        LastUpdated = DateTime.UtcNow,
        Status = PaymentTransactionStatus.Succeeded,
    };
}
