// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.Enums;
using PaymentGateway.Application.BillwerkSDK.DTO.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace PaymentGateway.Swagger.Preauth;

public class CancelPreauthResponseExample : IExamplesProvider<PaymentCancellationResponseDto>
{
    public PaymentCancellationResponseDto GetExamples() => new()
    {
        CancellationStatus = PaymentCancellationStatus.Succeeded,
        TransactionId = "20221118-636bd596c10cfc318b1bfabb",
    };
}
