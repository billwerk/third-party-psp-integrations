// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Payment;
using Billwerk.Payment.SDK.Enums;
using NodaTime;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Payment;

public class PaymentResponseExample : IExamplesProvider<PaymentResponseDto>
{
    public PaymentResponseDto GetExamples() => new()
    {
        TransactionId = "20221108-636a91f61c48d9fa186166f8",
        Status = PaymentTransactionStatus.Succeeded,
        RequestedAmount = 25m,
        Currency = "EUR",
        PspTransactionId = "cc4749ef-9b7f-42c0-b6e3-3e4ca5d4989c",
        ExternalTransactionId = "636d02197d50dda9cfb6a012",
        Bearer = new Dictionary<string, string>
        {
            { "PaymentRoleSpecificToken", "fbf538af-9bde-4f06-9ae7-442aff0b6881" }
        },
        Payments = new List<PaymentItemDto>
        {
            new()
            {
                ExternalItemId = "c141976a-705a-4ce4-99a1-af57b3cd1601",
                Amount = 25m,
                BookingDate = LocalDate.FromDateTime(DateTime.UtcNow),
                Description = "Successful full payment"
            }
        },
        RefundableAmount = 25m,
        RefundedAmount = 0m,
        LastUpdated = DateTime.UtcNow,
    };
}
